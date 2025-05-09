﻿using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace TelegramUpdater.RainbowUtilities;

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
    private readonly ConcurrentDictionary<TId, OwnerInfo<TId>> _ownerIdMapping;
    private readonly Func<TValue, TId> _idResolver;
    private readonly Func<ShiningInfo<TId, TValue>, CancellationToken, Task>? _handler;
    private readonly Func<Exception, CancellationToken, Task>? _exceptionHandler;
    private readonly ILogger<Rainbow<TId, TValue>> _logger;
    private readonly Action<Rainbow<TId, TValue>>? _gotIdle;
    private readonly TimeSpan _waitingListTimeOut;

    private Channel<TValue>? _waitingList;
    private Task? _waitlistTask = null;

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
    /// <param name="gotIdle">A callback function that will be called if
    /// <see cref="Rainbow{TId, TValue}"/> has nothing to do for now.
    /// </param>
    /// <param name="waitingListTimeout">Timeout to cancel waiting task if it's idle.</param>
    /// <param name="callback">A callback function to call for each object.</param>
    /// <param name="exceptionHandler">
    /// A callback function that will be called if any exceptions occurs in
    /// <paramref name="callback"/> method.
    /// </param>
    public Rainbow(
        int maximumParallel,
        Func<TValue, TId> idResolver,
        Func<ShiningInfo<TId, TValue>, CancellationToken, Task> callback,
        Func<Exception, CancellationToken, Task>? exceptionHandler = default,
        ILogger<Rainbow<TId, TValue>>? logger = default,
        Action<Rainbow<TId, TValue>>? gotIdle = default,
        TimeSpan waitingListTimeout = default)
    {
        _exceptionHandler = exceptionHandler;
        _handler = callback ?? throw new ArgumentNullException(nameof(callback));

#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumParallel);
#else
        if (maximumParallel <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumParallel));
#endif
        if (logger == null)
        {
            using var _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = _loggerFactory.CreateLogger<Rainbow<TId, TValue>>();
        }
        else
        {
            _logger = logger;
        }

        if (waitingListTimeout == default)
        {
            _logger.LogInformation("waitingListTimeout set to 30 sec.");
            _waitingListTimeOut = TimeSpan.FromSeconds(30);
        }
        else
        {
            _waitingListTimeOut = waitingListTimeout;
        }

        _idResolver = idResolver ?? throw new ArgumentNullException(nameof(idResolver));

        _availableQueues = new(maximumParallel + 1, maximumParallel);
        _workingTasks = new(maximumParallel + 1, maximumParallel);
        _ownerIdMapping = new(maximumParallel + 1, maximumParallel);
        _gotIdle = gotIdle;

        // Init available queues
        _logger.LogInformation(
            "Initializing {maximumParallel} queues in parallel.", maximumParallel);
        foreach (ushort number in Enumerable.Range(0, maximumParallel).Select(v => (ushort)v))
        {
            if (!_availableQueues.TryAdd(number, value: null))
            {
                throw new InvalidOperationException("?");
            }
        }
    }

    /// <summary>
    /// Write a new object to the rainbow for queuing
    /// </summary>
    public async ValueTask EnqueueAsync(
        TValue value, CancellationToken cancellationToken = default)
    {
        if (value == null)
            return;

        var ownerId = _idResolver(value);

        if (_ownerIdMapping.TryGetValue(ownerId, out OwnerInfo<TId> foundValue))
        {
            var alreadyQueue = _availableQueues[foundValue.QueueId];

            // Cannot be null here!
            await alreadyQueue!.Writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var queueId = GetAvailableQueue();

            // No empty queue
            if (queueId == null)
            {
                _logger.LogInformation("Queues are full! Object added to waiting list.");
                if (_waitlistTask == null)
                {
                    _waitingList = Channel.CreateUnbounded<TValue>(new UnboundedChannelOptions()
                    {
                        AllowSynchronousContinuations = true,
                        SingleReader = true,
                        SingleWriter = true,
                    });
                    _logger.LogInformation("Started a new waiting task.");

                    await _waitingList.Writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
                    _waitlistTask = WaitingQueuer(cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Object added to an existing waiting list...");
                }
                return;
            }

            var availableQueue = _availableQueues[queueId!.Value];

            if (availableQueue == null)
            {
                var ownerInfo = new OwnerInfo<TId>(ownerId, queueId.Value, DateTime.UtcNow);
                if (!_ownerIdMapping.TryAdd(ownerId, ownerInfo))
                {
                    _ownerIdMapping[ownerId] = ownerInfo;
                }

                var queue = Channel.CreateUnbounded<TValue>();
                await queue.Writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
                _availableQueues[queueId.Value] = queue;

                // ---- Create reading background task ----
                var t = Task.Run(() => Processor(queueId.Value, cancellationToken), cancellationToken);
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
                var ownerInfo = new OwnerInfo<TId>(ownerId, queueId.Value, DateTime.UtcNow);
                _ownerIdMapping.AddOrUpdate(ownerId, ownerInfo, (_, __) => ownerInfo);
                _logger.LogInformation(
                    "Queue {id}'s owner changed to {owner}", queueId.Value, ownerId);
                var t = Task.Run(() => Processor(queueId.Value, cancellationToken), cancellationToken);
                _workingTasks[queueId.Value] = t;

                await availableQueue.Writer.WriteAsync(value, cancellationToken).ConfigureAwait(false);
            }
        }
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
                .Select(x => new ProcessorInfo<TId, TValue>(
                    x.Key, GetQueueOwner(x.Key),
                    _workingTasks.GetValueOrDefault(x.Key)?.Status,
                    x.Value!.Reader.Count));
        }
    }

    /// <summary>
    /// Indicates if the rainbow is not doing anything noticeable.
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
            _logger.LogWarning("Queue {id} dropped manually!", queueId);

            return true;
        }

        return false;
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

        count = default;
        return false;
    }

    /// <summary>
    /// Tries to get next item in the queue.
    /// </summary>
    /// <param name="queueId">Queue id.</param>
    /// <param name="timeOut">Returns default on this timeout!</param>
    /// <param name="cancellationToken">Cancel the job.</param>
    public async ValueTask<ShiningInfo<TId, TValue>?> ReadNextAsync(
        ushort queueId,
        TimeSpan timeOut,
        CancellationToken cancellationToken = default)
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

        var currentOwner = GetQueueOwner(queueId) ?? throw new InvalidOperationException("An active queue with no owner?");
        var timeOutCts = new CancellationTokenSource();
        timeOutCts.CancelAfter(timeOut);

        using var linkedCts = CancellationTokenSource
            .CreateLinkedTokenSource(timeOutCts.Token, cancellationToken);

        try
        {
            var result = await channel.Reader.ReadAsync(linkedCts.Token).ConfigureAwait(false);

            var ownerAgain = GetQueueOwner(queueId);

            if (ownerAgain != null && ownerAgain.Value.OwnerId.Equals(currentOwner.OwnerId))
            {
                return new ShiningInfo<TId, TValue>(result, this, queueId);
            }

            return default;
        }
        catch (OperationCanceledException)
        {
            if (timeOutCts.IsCancellationRequested) return default;

            // Throw if it's canceled manually.
            throw;
        }
    }

    /// <summary>
    /// Yields all <typeparamref name="TValue"/>s from given <paramref name="queueId"/>.
    /// </summary>
    /// <param name="queueId">Queue id.</param>
    /// <param name="cancellationToken">Cancel the job.</param>
    public async IAsyncEnumerable<ShiningInfo<TId, TValue>> YieldAsync(
        ushort queueId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!_availableQueues.TryGetValue(queueId, out var channel)) yield break;

        if (channel == null) yield break;

        var currentOwner = GetQueueOwner(queueId);

        if (currentOwner == null) yield break;

        await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            yield return new ShiningInfo<TId, TValue>(item, this, queueId);
        }
    }

    /// <summary>
    /// Get an owner's queue.
    /// </summary>
    /// <param name="ownerId">Owner id</param>
    /// <returns>Returns null if there's no queue for this owner.</returns>
    public ushort? GetOwnersQueue(TId ownerId)
    {
        if (_ownerIdMapping.TryGetValue(ownerId, out OwnerInfo<TId> value))
        {
            return value.QueueId;
        }

        return default;
    }

    private ushort? GetAvailableQueue()
    {
        if (_availableQueues.Any(x => x.Value == null || x.Value.Reader.Count == 0))
        {
            var availableQueues = _availableQueues
                .Where(x => x.Value == null || x.Value.Reader.Count == 0);

            foreach (var queue in availableQueues)
            {
                if (_workingTasks.TryGetValue(queue.Key, out Task? task))
                {
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

        return null;
    }

    private async Task WaitingQueuer(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("---------- Waiting list Writer Started! ----------");

        // cancel if its idle for a long time.
        while (true)
        {
            var timeOutCts = new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource
                            .CreateLinkedTokenSource(timeOutCts.Token, cancellationToken);
            timeOutCts.CancelAfter(_waitingListTimeOut);

            try
            {
                TValue? update = await _waitingList!.Reader.ReadAsync(linkedCts.Token).ConfigureAwait(false);

                var ownerId = _idResolver(update);

                if (_ownerIdMapping.TryGetValue(ownerId, out OwnerInfo<TId> value)) // It's rare to happen here
                {
                    var alreadyQueue = _availableQueues[value.QueueId];

                    // Cannot be null here!
                    await alreadyQueue!.Writer.WriteAsync(update, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var queueId = GetAvailableQueue();

                    // No empty queue
                    if (queueId == null)
                    {
                        // block till any slots available
                        _logger.LogInformation("Waiting for a release...");
                        await WaitForFreeQueue().ConfigureAwait(false);

                        _logger.LogInformation("Free slots found.");
                        queueId = GetAvailableQueue();
                    }

                    var availableQueue = _availableQueues[queueId!.Value];

                    if (availableQueue == null)
                    {
                        var ownerInfo = new OwnerInfo<TId>(ownerId, queueId.Value, DateTime.UtcNow);

                        if (!_ownerIdMapping.TryAdd(ownerId, ownerInfo))
                        {
                            _ownerIdMapping[ownerId] = ownerInfo;
                        }

                        var queue = Channel.CreateUnbounded<TValue>();
                        await queue.Writer.WriteAsync(update, cancellationToken).ConfigureAwait(false);
                        _availableQueues[queueId.Value] = queue;

                        // ---- Create reading background task ----
                        var t = Task.Run(() => Processor(queueId.Value, cancellationToken), cancellationToken);
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
                        var ownerInfo = new OwnerInfo<TId>(ownerId, queueId.Value, DateTime.UtcNow);

                        _ownerIdMapping.AddOrUpdate(ownerId, ownerInfo, (_, __) => ownerInfo);
                        _logger.LogInformation(
                            "Queue {id}'s owner changed to {owner}", queueId.Value, ownerId);
                        var t = Task.Run(() => Processor(queueId.Value, cancellationToken), cancellationToken);
                        _workingTasks[queueId.Value] = t;

                        await availableQueue.Writer.WriteAsync(update, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                if (timeOutCts.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting task stopped due to a long idle.");
                    _waitlistTask = null;
                    _waitingList = null;
                }

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(exception: e, "Error while handling waiting list.");
            }
        }
    }

    private OwnerInfo<TId>? GetQueueOwner(ushort queueId)
    {
        try
        {
            return _ownerIdMapping!.First(x => x.Value.QueueId == queueId).Value;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private async Task WaitForFreeQueue()
    {
        var tasksToWait = new List<Task>();
        foreach (var task in _workingTasks)
        {
            if (task.Value != null)
            {
                if (!task.Value.IsCompleted)
                {
                    if (_availableQueues.TryGetValue(task.Key, out var channel))
                    {
                        if (channel != null)
                        {
                            if (channel.Reader.Count == 0)
                            {
                                // This is what i want to wait for
                                tasksToWait.Add(task.Value);
                            }
                        }
                    }
                }
            }
        }

        await Task.WhenAny(tasksToWait).ConfigureAwait(false);
    }

    private async Task Processor(
        ushort id, CancellationToken cancellationToken = default)
    {
        // Not null here!
        var myChannel = _availableQueues[id]! ?? throw new InvalidOperationException("?");
        
        while (true)
        {
            try
            {
                var update = await myChannel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    await _handler!(
                        new ShiningInfo<TId, TValue>(update, this, id), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (_exceptionHandler != null)
                    {
                        try
                        {
                            await _exceptionHandler(ex, cancellationToken).ConfigureAwait(false);
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
                        while (!_ownerIdMapping.TryRemove(owner.Value.OwnerId, out _))
                        { }

                        _logger.LogInformation("{owner} Is not {id}'owner anymore", owner.Value.OwnerId, id);
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
                        await _exceptionHandler(ex, cancellationToken).ConfigureAwait(false);
                    }
                    catch { }
                }
            }
        }
    }
}
