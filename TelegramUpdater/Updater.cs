using Microsoft.Extensions.DependencyInjection;
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
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.TrafficLights;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater
{
    /// <summary>
    /// Fetch updates from telegram and handle them.
    /// </summary>
    public class Updater : IUpdater
    {
        private readonly ITelegramBotClient _botClient;
        private readonly List<ISingletonUpdateHandler> _updateHandlers;
        private readonly List<IScopedHandlerContainer> _scopedHandlerContainers;
        private readonly List<IExceptionHandler> _exceptionHandlers;
        private readonly ConcurrentDictionary<string, IUpdateChannel> _updateChannels;
        private readonly Channel<Update> _updatesChannel; // i guess this is useless
        private readonly TrafficLight<Update, long> _trafficLight;
        private readonly ILogger<IUpdater> _logger;
        private readonly UpdaterOptions _updaterOptions;
        private readonly IServiceProvider? _serviceDescriptors;
        private readonly CancellationTokenSource _emergencyCts;
        private User? _me = null;

        /// <summary>
        /// Creates an instance of updater to fetch updates from telegram and handle them.
        /// </summary>
        /// <param name="botClient">Telegram bot client</param>
        /// <param name="updaterOptions">Options for this updater.</param>
        /// <param name="serviceDescriptors">Optional service provider.</param>
        public Updater(
            ITelegramBotClient botClient,
            UpdaterOptions updaterOptions = default,
            IServiceProvider? serviceDescriptors = default)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _updaterOptions = updaterOptions;
            _serviceDescriptors = serviceDescriptors;
            _emergencyCts = new CancellationTokenSource();

            _updateChannels = new ConcurrentDictionary<string, IUpdateChannel>();
            _updateHandlers = new List<ISingletonUpdateHandler>();
            _exceptionHandlers = new List<IExceptionHandler>();
            _scopedHandlerContainers = new List<IScopedHandlerContainer>();
            _trafficLight = new TrafficLight<Update, long>(x => x.GetSenderId() ?? 0, updaterOptions.MaxDegreeOfParallelism);

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

        /// <inheritdoc/>
        public UpdaterOptions UpdaterOptions => _updaterOptions;

        /// <inheritdoc/>
        public CancellationToken EmergencyToken => _emergencyCts.Token;

        /// <inheritdoc/>
        public ITelegramBotClient BotClient => _botClient;

        /// <inheritdoc/>
        public ILogger<IUpdater> Logger => _logger;

        /// <inheritdoc/>
        public TrafficLight<Update, long> TrafficLight => _trafficLight;

        /// <inheritdoc/>
        public ChannelReader<Update> ChannelReader => _updatesChannel.Reader;

        /// <inheritdoc/>
        public ChannelWriter<Update> ChannelWriter => _updatesChannel.Writer;

        /// <inheritdoc/>
        public Updater AddUpdateHandler(ISingletonUpdateHandler updateHandler)
        {
            _updateHandlers.Add(updateHandler);
            _logger.LogInformation($"Added new singleton handler.");
            return this;
        }

        /// <inheritdoc/>
        public Updater AddScopedHandler(IScopedHandlerContainer scopedHandlerContainer)
        {
            var _h = scopedHandlerContainer.GetType();
            _scopedHandlerContainers.Add(scopedHandlerContainer);
            _logger.LogInformation($"Added new scoped handler :: {_h.Name}.");
            return this;
        }

        /// <inheritdoc/>
        public Updater AddExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
            _logger.LogInformation($"Added exception handler for {exceptionHandler.ExceptionType}");
            return this;
        }

        /// <inheritdoc/>
        public async Task<IContainer<T>?> OpenChannel<T>(AbstractChannel<T> updateChannel, TimeSpan timeOut)
            where T : class
        {
            var key = updateChannel.GetHashCode().ToString();
            if (_updateChannels.TryAdd(
                key, updateChannel))
            {
                try
                {
                    return await updateChannel.ReadAsync(timeOut);
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

        /// <inheritdoc/>
        public async Task Start(
            bool block = true,
            bool manualWriting = false,
            bool fromServices = false,
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

                    if (_emergencyCts.IsCancellationRequested)
                    {
                        _logger.LogCritical("Update reader stopped due to emergency cancel request.");
                        break;
                    }

                    var update = await _updatesChannel.Reader.ReadAsync(_emergencyCts.Token);

                    if (update == null)
                        continue;

                    // This is an overall wait
                    await _trafficLight.AwaitGreenLight();

                    if (fromServices)
                    {
                        _ = ProcessUpdateFromServices(update, cancellationToken);
                    }
                    else
                    {
                        _ = ProcessUpdate(update, cancellationToken);
                    }
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

        /// <inheritdoc/>
        public async Task<User> GetMeAsync()
        {
            if (_me == null)
            {
                _me = await _botClient.GetMeAsync();
            }

            return _me;
        }

        /// <inheritdoc/>
        public void Cancel() => _emergencyCts.Cancel();

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
                try
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
                catch (Exception e)
                {
                    Logger.LogCritical(exception: e, "Auto update writer stopped due to an ecxeption.");
                    _emergencyCts.Cancel();
                    break;
                }
            }
        }

        private async Task ProcessUpdateFromServices(
            Update update, CancellationToken cancellationToken)
        {
            try
            {

                if (_serviceDescriptors == null)
                    throw new InvalidOperationException("Can't ProcessUpdateFromServices when there is no ServiceProvider.");

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (!_updateChannels.IsEmpty)
                {
                    IUpdateChannel? updateChannel = null;
                    string? key = null;

                    foreach (var channel in _updateChannels
                        .Where(x => x.Value.UpdateType == update.Type))
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
                            await updateChannel.WriteAsync(this, update);
                        }

                        _updateChannels.Remove(key!, out _);
                        updateChannel.Dispose();
                        return;
                    }
                }

                var scopedHandlers = _scopedHandlerContainers
                    .Where(x => x.UpdateType == update.Type)
                    .Where(x => x.ShouldHandle(update));

                if (scopedHandlers.Any())
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
                foreach (var container in scopedHandlers)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    using var scope = _serviceDescriptors.CreateScope();

                    var handler = container.CreateInstance(scope);

                    if (handler != null)
                    {
                        if (!await HandleHandler(update, handler, cancellationToken))
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(exception: e, "Error in ProcessUpdateFromServices.");
            }
        }

        private async Task ProcessUpdate(
            Update update, CancellationToken cancellationToken)
        {
            try
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
                        .Where(x => x.Value.UpdateType == update.Type))
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
                            await updateChannel.WriteAsync(this, update);
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
                    .Select(x => x.CreateInstance())
                    .Where(x => x != null)
                    .Cast<IScopedUpdateHandler>()
                    .Select(x => (IUpdateHandler)x);

                var handlers = singletonhandlers.Concat(scopedHandlers)
                    .OrderBy(x => x.Group);

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
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, "Error in ProcessUpdate.");
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
                    update, handler.HandleAsync(this, update));
            }
            else
            {
                crossingInfo = _trafficLight.StartCrossingTask(
                    handler.HandleAsync(this, update));
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
                    .Where(x => x.TypeIsMatched(ex.GetType()))
                    .Where(x => x.IsAllowedHandler(handler.GetType()))
                    .Where(x => x.MessageMatched(ex.Message));

                foreach (var exHandler in exHandlers)
                {
                    await exHandler.Callback(this, ex);
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