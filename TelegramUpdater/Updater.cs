using System.Collections.Concurrent;
using System.Threading.Channels;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.TrafficLights;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater
{
    /// <summary>
    /// Fetch updates from telegram and handle them.
    /// </summary>
    public class Updater
    {
        private readonly ITelegramBotClient _botClient;
        private readonly List<ISingletonUpdateHandler> _updateHandlers;
        private readonly List<IScopedHandlerContainer> _scopedHandlerContainers;
        private readonly List<IExceptionHandler> _exceptionHandlers;
        private readonly ConcurrentDictionary<string, IUpdateChannel> _updateChannels;
        private readonly Channel<Update> _updatesChannel; // i guess this is useless
        private Thread? _updaterThread;
        private readonly TrafficLight<Update, long> _trafficLight;
        private readonly bool _perUserOneByOneProcess;

        /// <summary>
        /// Creates an instance of updater to fetch updates from telegram and handle them.
        /// </summary>
        /// <param name="botClient">Telegram bot client</param>
        /// <param name="maxDegreeOfParallelism">The maximum number of concurrent update handling tasks</param>
        /// <param name="perUserOneByOneProcess">If updates should process one by one per user.</param>
        public Updater(
            ITelegramBotClient botClient,
            int? maxDegreeOfParallelism = default,
            bool perUserOneByOneProcess = true)
        {
            _botClient = botClient;
            _updateChannels = new();
            _updateHandlers = new();
            _exceptionHandlers = new();
            _scopedHandlerContainers = new();
            _trafficLight = new(x=> x.GetSenderId() ?? 0, maxDegreeOfParallelism);
            _perUserOneByOneProcess= perUserOneByOneProcess;

            _updatesChannel = Channel.CreateBounded<Update>(
                new BoundedChannelOptions(100)
                {
                    SingleWriter = true,
                    AllowSynchronousContinuations = true,
                    FullMode = BoundedChannelFullMode.Wait
                });
        }

        /// <summary>
        /// A dict of tasks for in process updates
        /// </summary>
        public TrafficLight<Update, long> TrafficLight => _trafficLight;

        /// <summary>
        /// You can read updates from here.
        /// </summary>
        public ChannelReader<Update> ChannelReader => _updatesChannel.Reader;

        /// <summary>
        /// Add your handler to this updater.
        /// </summary>
        /// <param name="updateHandler"></param>
        public void AddUpdateHandler(ISingletonUpdateHandler updateHandler)
        {
            _updateHandlers.Add(updateHandler);
        }

        /// <summary>
        /// Adds an scoped handler to the updater.
        /// </summary>
        /// <typeparam name="THandler">Handler type.</typeparam>
        /// <typeparam name="TUpdate">Update type.</typeparam>
        /// <param name="updateType">Update type again.</param>
        /// <param name="filter">A filter to choose the right update.</param>
        /// <param name="getT">
        /// A function to choose real update from <see cref="Update"/>
        /// <para>Don't touch it if you don't know.</para>
        /// </param>
        public void AddScopedHandler<THandler, TUpdate>(
            Filter<TUpdate>? filter = default,
            UpdateType? updateType = default,
            Func<Update, TUpdate>? getT = default)
            where THandler : IScopedUpdateHandler where TUpdate : class
        {
            if (updateType == null)
            {
                var _t = typeof(TUpdate);
                if (Enum.TryParse(_t.Name, out UpdateType ut))
                {
                    updateType = ut;
                }
                else
                {
                    throw new InvalidCastException($"{_t} is not an Update, Should Message, CallbackQuery, ...");
                }
            }

            _scopedHandlerContainers.Add(
                new UpdateContainerBuilder<THandler, TUpdate>(
                    updateType.Value, filter, getT));
        }

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="exceptionHandler"></param>
        public void AddExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
        }

        /// <summary>
        /// Opens a channel through the update handler and reads specified update.
        /// </summary>
        /// <typeparam name="T">Type of update you're excepting.</typeparam>
        /// <param name="updateChannel">An <see cref="IUpdateChannel"/></param>
        /// <param name="timeOut">Maximum allowed time to wait for that update.</param>
        public async Task<T?> OpenChannel<T>(AbstractChannel<T> updateChannel, TimeSpan timeOut)
            where T: class
        {
            var key = updateChannel.GetHashCode().ToString();
            if (_updateChannels.TryAdd(
                key, updateChannel))
            {
                try
                {
                    return updateChannel.GetT(await updateChannel.ReadAsync(timeOut));
                }
                catch (OperationCanceledException)
                {
                    _updateChannels.Remove(key!, out _);
                    updateChannel.Dispose();
                    return null;
                }
            }
            else
            {
                throw new Exception("Can't open channel!");
            }
        }

        /// <summary>
        /// Start handling updates.
        /// </summary>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        public async ValueTask Start(
            CancellationToken cancellationToken = default)
        {
            _updaterThread = new Thread(async () => await UpdateReceiver());
            _updaterThread.Start();

            while (true)
            {
                var update = await _updatesChannel.Reader.ReadAsync(cancellationToken);

                // This is an overall wait
                await _trafficLight.AwaitGreenLight();

                _ = ProcessUpdate(update, cancellationToken);
            }
        }

        private async Task UpdateReceiver(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Started update receiver.");

            var offset = 0;
            var timeOut = 1000;

            while (true)
            {
                var updates = await _botClient.GetUpdatesAsync(offset, 100, timeOut,
                                                               cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    await _updatesChannel.Writer.WriteAsync(update, cancellationToken);
                    offset = update.Id + 1;
                }
            }
        }

        private async Task ProcessUpdate(Update update, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (!_updateChannels.IsEmpty)
            {
                IUpdateChannel? updateChannel = null;
                string? key = null;

                foreach (var channel in _updateChannels
                    .Where(x=> x.Value.UpdateType == update.Type))
                {
                    // TODO: use UpdateType to choose wisely
                    if (channel.Value.ShouldChannel(update))
                    {
                        updateChannel = channel.Value;
                        key = channel.Key;
                        break;
                    }
                }

                if (updateChannel != null)
                {
                    if (!updateChannel.Cancelled)
                    {
                        await updateChannel.WriteAsync(update);
                    }

                    _updateChannels.Remove(key!, out _);
                    updateChannel.Dispose();
                    return;
                }
            }

            var singletonhandlers = _updateHandlers
                .Where(x => x.UpdateType == update.Type)
                .Where(x => x.ShouldHandle(update))
                .Select(x => (IUpdateHandler)x);

            var scopedHandlers = _scopedHandlerContainers
                .Where(x => x.UpdateType == update.Type)
                .Where(x => x.ShouldHandle(update))
                .Select(x=> x.CreateInstance())
                .Where(x=> x is not null)
                .Cast<IScopedUpdateHandler>()
                .Select(x=> (IUpdateHandler)x);

            var handlers = singletonhandlers.Concat(scopedHandlers)
                .OrderBy(x=> x.Group);

            if (handlers.Any() || scopedHandlers.Any())
            {
                if (_perUserOneByOneProcess)
                {
                    // This is a per container wait ( eg: per user )
                    await _trafficLight.AwaitYellowLight(update);
                }
            }
            else
            {
                return;
            }

            // valid handlers for an update should process one by one
            // This change can be cut off when throwing an specified exception
            // Other exceptions are redirected to ExceptionHandler.
            foreach (var handler in handlers)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (!await HandleHandler(update, handler, cancellationToken))
                {
                    break;
                }
            }
        }
    
        /// <summary>
        /// Returns false to break.
        /// </summary>
        private async Task<bool> HandleHandler(
            Update update,
            IUpdateHandler handler,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                if (_perUserOneByOneProcess)
                {
                    await _trafficLight.CreateCrossingTask(
                        update, handler.HandleAsync(this, _botClient, update));
                }
                else
                {
                    await _trafficLight.CreateCrossingTask(
                        handler.HandleAsync(this, _botClient, update));
                }
            }
            // Cut handlers chain.
            catch (StopPropagation)
            {
                return false;
            }
            catch (ContinuePropagation)
            {
                return true;
            }
            catch (Exception ex)
            {
                // Do exception handlers
                var exHandlers = _exceptionHandlers
                    .Where(x => x.ExceptionType == ex.GetType())
                    .Where(x => x.MessageMatched(ex.Message));

                foreach (var exHandler in exHandlers)
                {
                    await exHandler.Callback(ex);
                }
            }

            return true;
        }
    }
}