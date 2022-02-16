using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TelegramUpdater.RainbowUtlities
{
    /// <summary>
    /// Queues different objects in parallel.
    /// </summary>
    /// <remarks>
    /// Did you ever see a rainbow? constructed from a set of parallel lines with different colors.
    /// </remarks>
    /// <typeparam name="TId">The object's id.</typeparam>
    /// <typeparam name="TValue">Object's type</typeparam>
    public sealed class Rainbow<TId, TValue> where TId : struct
    {
        private readonly ConcurrentDictionary<ushort, Channel<TValue>?> _availableQueues;
        private readonly ConcurrentDictionary<ushort, Task?> _workingTasks;
        private readonly ConcurrentDictionary<TId, ushort> _ownerIdMapping;
        private readonly Channel<TValue> _mainChannel;
        private readonly Func<TValue, TId> _idResolver;
        private Func<ShiningInfo<TId, TValue>, CancellationToken, Task>? _handler;
        private Func<Exception, CancellationToken, Task>? _exceptionHandler;
        private readonly ILogger<Rainbow<TId, TValue>> _logger;
        private readonly Action<Rainbow<TId, TValue>>? _gotIdle;
        private bool _waitingForSlote = false;

        /// <summary>
        /// Queues different objects in parallel.
        /// </summary>
        /// <remarks>
        /// Did you ever see a rainbow? constructed from a set of parallel lines with different colors.
        /// </remarks>
        /// <param name="maximumParallel">Maximum allowed queues which are supposed to be in parallel.</param>
        /// <param name="idResolver">
        /// A function to find every objects id.
        /// <para>
        /// Objects with the same id are queued together.
        /// </para>
        /// </param>
        /// <param name="logger">A logger.</param>
        public Rainbow(
            int maximumParallel,
            Func<TValue, TId> idResolver,
            ILogger<Rainbow<TId, TValue>>? logger = default,
            Action<Rainbow<TId, TValue>>? gotIdle = default)
        {
            if (maximumParallel <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximumParallel));

            _idResolver = idResolver ?? throw new ArgumentNullException(nameof(idResolver));

            _availableQueues = new(maximumParallel + 1, maximumParallel);
            _workingTasks = new(maximumParallel + 1, maximumParallel);
            _ownerIdMapping = new(maximumParallel + 1, maximumParallel);
            _gotIdle = gotIdle;
            _mainChannel = Channel.CreateBounded<TValue>(new BoundedChannelOptions(100)
            {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });

            if (logger == null)
            {
                using var _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                _logger = _loggerFactory.CreateLogger<Rainbow<TId, TValue>>();
            }
            else
            {
                _logger = logger;
            }

            // Init avaiable queues
            _logger.LogInformation(
                "Initializing {maximumParallel} queues in parallel.", maximumParallel);
            foreach (ushort number in Enumerable.Range(0, maximumParallel))
            {
                if (!_availableQueues.TryAdd(number, null))
                {
                    throw new InvalidOperationException("?");
                }
            }
        }

        /// <summary>
        /// Starts shining ( blocking ).
        /// </summary>
        /// <param name="callback">A callback function to call for each object.</param>
        /// <returns></returns>
        [MemberNotNull("_handler")]
        public async Task ShineAsync(
            Func<ShiningInfo<TId, TValue>, CancellationToken, Task> callback,
            Func<Exception, CancellationToken, Task>? exceptionHandler = default,
            CancellationToken cancellationToken = default)
        {
            _exceptionHandler = exceptionHandler;
            _handler = callback ?? throw new ArgumentNullException(nameof(callback));

            _logger.LogInformation("Started main queuer task.");
            await MainQueuer(cancellationToken);
        }

        /// <summary>
        /// Write a new object to the rainbow for queuing
        /// </summary>
        public async Task WriteAsync(TValue value, CancellationToken cancellationToken = default)
        {
            if (value == null)
                return;

            await _mainChannel.Writer.WriteAsync(value, cancellationToken);
        }

        /// <summary>
        /// Get info about current processes and queues.
        /// </summary>
        /// <remarks>
        /// Only processes that have any pending objects.
        /// </remarks>
        public IEnumerable<ProcessorInfo<TId, TValue>> Processes
        {
            get
            {
                return _availableQueues
                    .Where(x => x.Value != null)
                    .Where(x => x.Value!.Reader.Count > 0)
                    .Select(x => new ProcessorInfo<TId, TValue>(
                        x.Key, GetQueueOwner(x.Key),
                        _workingTasks.GetValueOrDefault(x.Key)?.Status,
                        x.Value!.Reader.Count));
            }
        }

        /// <summary>
        /// Indicates if the rainbow is not doing anything noteabel.
        /// </summary>
        public bool IsIdle =>
            _availableQueues.All(x => x.Value == null || x.Value.Reader.Count == 0) &&
            _workingTasks.All(x => x.Value == null || x.Value.Status == TaskStatus.RanToCompletion);

        /// <summary>
        /// Tries to drop all pending objects for a given queue.
        /// </summary>
        /// <param name="queueId">Queue id.</param>
        /// <returns></returns>
        public bool DropPendingAsync(ushort queueId)
        {
            if (_availableQueues.TryGetValue(queueId, out var queued))
            {
                if (queued == null)
                {
                    return false;
                }

                _availableQueues[queueId] = null;
                _logger.LogWarning("Queue {id} droped manually!", queueId);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to count pending objects of the queue.
        /// </summary>
        /// <param name="queueId">Queue id.</param>
        /// <param name="count">Returned count of the queue.</param>
        /// <returns></returns>
        public bool TryCountPending(ushort queueId, [NotNullWhen(true)] out int? count)
        {
            if (_availableQueues.TryGetValue(queueId, out var channel))
            {
                if (channel == null)
                {
                    count = default;
                    return false;
                }

                count = channel.Reader.Count;
                return true;
            }
            else
            {
                count = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to get next item in the queue.
        /// </summary>
        /// <param name="queueId">Queue id.</param>
        /// <param name="timeOut">Returns default on this timeout!</param>
        public async ValueTask<ShiningInfo<TId, TValue>?> ReadNextAsync(
            ushort queueId, TimeSpan timeOut, CancellationToken cancellationToken = default)
        {
            if (!_availableQueues.TryGetValue(queueId, out var channel))
            {
                return default;
            }

            if (channel == null)
            {
                return default;
            }

            // If there's no items in the channel
            // or there is no running tasks on this queue
            // return default
            if (channel.Reader.Count == 0)
            {
                if (_workingTasks.TryGetValue(queueId, out var task))
                {
                    if (task == null) return default;

                    if (task.Status == TaskStatus.RanToCompletion) return default;
                }
            }

            var currentOwner = GetQueueOwner(queueId);

            if (currentOwner == null)
                throw new InvalidOperationException("An active queue with no owner?");

            var timeOutCts = new CancellationTokenSource();
            timeOutCts.CancelAfter(timeOut);

            using var linkedCts = CancellationTokenSource
                .CreateLinkedTokenSource(timeOutCts.Token, cancellationToken);

            try
            {
                var result = await channel.Reader.ReadAsync(linkedCts.Token);

                var ownerAgain = GetQueueOwner(queueId);

                if (ownerAgain != null && ownerAgain.Value.Equals(currentOwner.Value))
                {
                    return new ShiningInfo<TId, TValue>(result, this, queueId);
                }
                else
                {
                    return default;
                }
            }
            catch (OperationCanceledException)
            {
                if (timeOutCts.IsCancellationRequested) return default;

                // Throw if it's cancelled manually.
                throw;
            }
        }

        private ushort? GetAvailableQueue()
        {
            if (_availableQueues.Any(x => x.Value == null || x.Value.Reader.Count == 0))
            {
                var availableQueues = _availableQueues
                    .Where(x => x.Value == null || x.Value.Reader.Count == 0);

                foreach (var queue in availableQueues)
                {
                    if (_workingTasks.ContainsKey(queue.Key))
                    {
                        var task = _workingTasks[queue.Key];

                        if (task != null)
                        {
                            // Ready to add!
                            if (task.Status == TaskStatus.RanToCompletion)
                            {
                                return queue.Key;
                            }
                        }
                        else
                        {
                            // Ready to add!
                            return queue.Key;
                        }
                    }
                    else
                    {
                        // Ready to add!
                        return queue.Key;
                    }
                }

                return null;
            }
            else
            {
                return null;
            }
        }

        private async Task MainQueuer(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("---------- Writer Started! ----------");

            while (true)
            {
                TValue? update = await _mainChannel.Reader.ReadAsync(cancellationToken);

                var ownerId = _idResolver(update);

                if (_ownerIdMapping.ContainsKey(ownerId))
                {
                    var alreadyQueue = _availableQueues[_ownerIdMapping[ownerId]];

                    // Cannot be null here!
                    await alreadyQueue!.Writer.WriteAsync(update, cancellationToken);
                }
                else
                {
                    var queueId = GetAvailableQueue();

                    // No empty queue
                    if (queueId == null)
                    {
                        _logger.LogInformation("Queues are full! waiting for a free slote.");
                        _waitingForSlote = true;

                        while (_waitingForSlote)
                        {
                            await Task.Delay(500, cancellationToken);
                        }

                        _logger.LogInformation("Found a slote!");
                        queueId = GetAvailableQueue();
                    }

                    var availableQueue = _availableQueues[queueId!.Value];

                    if (availableQueue == null)
                    {
                        if (!_ownerIdMapping.TryAdd(ownerId, queueId.Value))
                        {
                            _ownerIdMapping[ownerId] = queueId.Value;
                        };

                        var queue = Channel.CreateUnbounded<TValue>();
                        await queue.Writer.WriteAsync(update, cancellationToken);
                        _availableQueues[queueId.Value] = queue;

                        // ---- Create reading background task ----
                        var t = Task.Run(() => Processor(queueId.Value), cancellationToken);
                        if (!_workingTasks.TryAdd(queueId.Value, t))
                        {
                            _workingTasks[queueId.Value] = t;
                        }

                        _logger.LogInformation(
                            "Created a new processor ({id}), Owned by {owner}",
                            queueId.Value, ownerId);
                    }
                    else
                    {
                        _ownerIdMapping.AddOrUpdate(ownerId, queueId.Value, (_, __) => queueId.Value);
                        _logger.LogInformation(
                            "Queue {id}'s owner changed to {owner}", queueId.Value, ownerId);
                        var t = Task.Run(() => Processor(queueId.Value), cancellationToken);
                        _workingTasks[queueId.Value] = t;

                        await availableQueue.Writer.WriteAsync(update, cancellationToken);
                    }
                }
            }
        }

        private TId? GetQueueOwner(ushort queueId)
        {
            try
            {
                return _ownerIdMapping!.First(x => x.Value == queueId).Key;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private async Task Processor(ushort id, CancellationToken cancellationToken = default)
        {
            // Not null here!
            var myChannel = _availableQueues[id]!;

            if (myChannel == null)
            {
                throw new InvalidOperationException("?");
            }

            while (true)
            {
                try
                {
                    var update = await myChannel.Reader.ReadAsync(cancellationToken);

                    try
                    {
                        await _handler!(
                            new ShiningInfo<TId, TValue>(update, this, id), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        if (_exceptionHandler != null)
                        {
                            try
                            {
                                await _exceptionHandler(ex, cancellationToken);
                            }
                            catch { }
                        }
                    }

                    if (myChannel.Reader.Count == 0)
                    {
                        var owner = GetQueueOwner(id);
                        if (owner != null)
                        {
                            // Make sure the owner is removed
                            while (!_ownerIdMapping.TryRemove(owner.Value, out _))
                            { }
                        }

                        _logger.LogInformation("{0owner} Is not {id}'owner anymore", owner, id);
                        if (_waitingForSlote)
                        {
                            _waitingForSlote = false;
                        }

                        if (IsIdle)
                        {
                            _logger.LogInformation("No more updates for now!");
                            _gotIdle?.Invoke(this);
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(exception: ex, message: "Error while handling.");
                    if (_exceptionHandler != null)
                    {
                        try
                        {
                            await _exceptionHandler(ex, cancellationToken);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
