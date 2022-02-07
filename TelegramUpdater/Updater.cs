using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
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
        private readonly TrafficLight<Update, long> _trafficLight;
        private readonly ILogger<Updater> _logger;
        private readonly UpdaterOptions _updaterOptions;

        /// <summary>
        /// Creates an instance of updater to fetch updates from telegram and handle them.
        /// </summary>
        /// <param name="botClient">Telegram bot client</param>
        /// <param name="updaterOptions">Options for this updater.</param>
        public Updater(
            ITelegramBotClient botClient,
            UpdaterOptions updaterOptions = default)
        {
            _botClient = botClient?? throw new ArgumentNullException(nameof(botClient));
            _updaterOptions = updaterOptions;

            _updateChannels = new ConcurrentDictionary<string, IUpdateChannel>();
            _updateHandlers = new List<ISingletonUpdateHandler>();
            _exceptionHandlers = new List<IExceptionHandler>();
            _scopedHandlerContainers = new List<IScopedHandlerContainer>();
            _trafficLight = new TrafficLight<Update, long>(x=> x.GetSenderId() ?? 0, updaterOptions.MaxDegreeOfParallelism);

            _updatesChannel = Channel.CreateBounded<Update>(
                new BoundedChannelOptions(100)
                {
                    SingleWriter = true,
                    AllowSynchronousContinuations = true,
                    FullMode = BoundedChannelFullMode.Wait
                });

            if (_updaterOptions.Logger == null)
            {
                using var _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();
                });

                _logger = _loggerFactory.CreateLogger<Updater>();
            }
            else
            {
                _logger = _updaterOptions.Logger;
            }

            _logger.LogInformation("Logger initialized.");
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
        /// You can read updates from here.
        /// </summary>
        public ChannelWriter<Update> ChannelWriter => _updatesChannel.Writer;

        /// <summary>
        /// Add your handler to this updater.
        /// </summary>
        /// <param name="updateHandler"></param>
        public void AddUpdateHandler(ISingletonUpdateHandler updateHandler)
        {
            _updateHandlers.Add(updateHandler);
            _logger.LogInformation($"Added new singleton handler.");
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
                    throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
                }
            }

            var _h = typeof(THandler);

            if (filter == null)
            {
                // If no filter passed as method args the look at attributes
                // Attribute filters are all combined using & operator.

                var applied = _h.GetCustomAttributes(typeof(ApplyFilterAttribute), false);
                foreach (ApplyFilterAttribute item in applied)
                {
                    var f = (Filter<TUpdate>?)Activator.CreateInstance(item.FilterType);
                    if (f != null)
                    {
                        if(filter == null)
                        {
                            filter = f;
                        }
                        else
                        {
                            filter &= f;
                        }
                    }
                }
            }

            _scopedHandlerContainers.Add(
                new UpdateContainerBuilder<THandler, TUpdate>(
                    updateType.Value, filter, getT));
            _logger.LogInformation($"Added new scoped {updateType} handler :: {_h.Name}.");
        }

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="exceptionHandler"></param>
        public void AddExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
            _logger.LogInformation($"Added exception handler for {exceptionHandler.ExceptionType}");
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
        /// <param name="block">If this method should block the thread.</param>
        /// <param name="manualWriting">If you gonna write updates manually and no polling required.</param>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        public async Task Start(
            bool block = true,
            bool manualWriting = false,
            CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
            {
                _logger.LogInformation("Start's CancellationToken set to CancellationToken in UpdaterOptions");
                cancellationToken = _updaterOptions.CancellationToken;
            }

            if (manualWriting)
            {
                _logger.LogWarning("Manual writing is enabled! You should write updates yourself.");
            }
            else
            {
                _logger.LogInformation("Auto writing updates enabled!");
                var updaterTask = Task.Run(() => UpdateReceiver(cancellationToken), cancellationToken);
            }

            var readerTask = Task.Run(async () =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Reading updates cancelled!");
                        break;
                    }

                    var update = await _updatesChannel.Reader.ReadAsync(cancellationToken);

                    // This is an overall wait
                    await _trafficLight.AwaitGreenLight();

                    _ = ProcessUpdate(update, cancellationToken);
                }
            }, cancellationToken);

            if (block)
            {
                _logger.LogInformation("Blocking the current thread to read updates!");
                await readerTask;
            }
            else
            {
                _logger.LogInformation("Reading updates is done in background.");
            }
        }

        private async Task UpdateReceiver(CancellationToken cancellationToken = default)
        {
            if (_updaterOptions.FlushUpdatesQueue)
            {
                _logger.LogInformation("Flushing updates.");
                await _botClient.GetUpdatesAsync(-1, 1, 0, cancellationToken: cancellationToken);
            }

            var offset = 0;
            var timeOut = 1000;

            _logger.LogInformation("Started Polling.");

            while (true)
            {
                var updates = await _botClient.GetUpdatesAsync(offset, 100, timeOut,
                                                               allowedUpdates: _updaterOptions.AllowedUpdates,
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
                .Where(x=> x != null)
                .Cast<IScopedUpdateHandler>()
                .Select(x=> (IUpdateHandler)x);

            var handlers = singletonhandlers.Concat(scopedHandlers)
                .OrderBy(x=> x.Group);

            if (handlers.Any() || scopedHandlers.Any())
            {
                if (_updaterOptions.PerUserOneByOneProcess)
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

            // Create and setup handling task stuff
            CrossingInfo<long> crossingInfo;
            if (_updaterOptions.PerUserOneByOneProcess)
            {
                crossingInfo = _trafficLight.StartCrossingTask(
                    update, handler.HandleAsync(this, _botClient, update));
            }
            else
            {
                crossingInfo = _trafficLight.StartCrossingTask(
                    handler.HandleAsync(this, _botClient, update));
            }

            // Handle the shit.
            try
            {
                await crossingInfo.UnderlyingTask;
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
                    .Where(x => x.IsAllowedHandler(handler.GetType()))
                    .Where(x => x.MessageMatched(ex.Message));

                foreach (var exHandler in exHandlers)
                {
                    await exHandler.Callback(ex);
                }
            }
            finally
            {
                // Cleanup handling information.
                _trafficLight.FinishCrossing(crossingInfo);

                if (handler is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return true;
        }
    }
}