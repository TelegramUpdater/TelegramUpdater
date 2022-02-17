using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.RainbowUtlities;
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
        private readonly ILogger<IUpdater> _logger;
        private readonly UpdaterOptions _updaterOptions;
        private readonly IServiceProvider? _serviceDescriptors;
        private readonly Rainbow<long, Update> _rainbow;
        private CancellationTokenSource? _emergencyCancel;
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

            _updateHandlers = new List<ISingletonUpdateHandler>();
            _exceptionHandlers = new List<IExceptionHandler>();
            _scopedHandlerContainers = new List<IScopedHandlerContainer>();

            _rainbow = new Rainbow<long, Update>(
                updaterOptions.MaxDegreeOfParallelism?? Environment.ProcessorCount,
                x => x.GetSenderId() ?? 0,
                ShineCallback, ShineErrors);

            if (_updaterOptions.Logger == null)
            {
                using var _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
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
        public ITelegramBotClient BotClient => _botClient;

        /// <inheritdoc/>
        public ILogger<IUpdater> Logger => _logger;

        /// <inheritdoc/>
        public Rainbow<long, Update> Rainbow => _rainbow;

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
            _logger.LogInformation("Added new scoped handler :: {Name}.", _h.Name);
            return this;
        }

        /// <inheritdoc/>
        public Updater AddExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
            _logger.LogInformation(
                "Added exception handler for {ExceptionType}",
                exceptionHandler.ExceptionType);
            return this;
        }

        /// <inheritdoc/>
        public async Task WriteAsync(Update update, CancellationToken cancellationToken)
        {
            await Rainbow.EnqueueAsync(update, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
            {
                _logger.LogInformation("Start's CancellationToken set to CancellationToken in UpdaterOptions");
                cancellationToken = _updaterOptions.CancellationToken;
            }

            _logger.LogInformation("Auto writing updates enabled!");

            _emergencyCancel = new CancellationTokenSource();
            using var liked = CancellationTokenSource.CreateLinkedTokenSource(
                _emergencyCancel.Token, cancellationToken);

            var updaterTask = Task.Run(() => UpdateReceiver(liked.Token), liked.Token);

            _logger.LogInformation("Blocking the current thread to read updates!");
            await _rainbow.ShineAsync(ShineCallback, ShineErrors, liked.Token);
        }

        /// <inheritdoc/>
        public async Task StartReaderOnlyAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
            {
                _logger.LogInformation("Start's CancellationToken set to CancellationToken in UpdaterOptions");
                cancellationToken = _updaterOptions.CancellationToken;
            }

            _logger.LogWarning("Manual writing is enabled! You should write updates yourself.");

            _emergencyCancel = new CancellationTokenSource();
            using var liked = CancellationTokenSource.CreateLinkedTokenSource(
                _emergencyCancel.Token, cancellationToken);

            _logger.LogInformation("Blocking the current thread to read updates!");
            await _rainbow.ShineAsync(ShineCallback, ShineErrors, liked.Token);
        }

        /// <inheritdoc/>
        public void Start(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
            {
                _logger.LogInformation("Start's CancellationToken set to CancellationToken in UpdaterOptions");
                cancellationToken = _updaterOptions.CancellationToken;
            }

            _logger.LogInformation("Auto writing updates enabled!");

            _emergencyCancel = new CancellationTokenSource();
            using var liked = CancellationTokenSource.CreateLinkedTokenSource(
                _emergencyCancel.Token, cancellationToken);
            var updaterTask = Task.Run(() => UpdateReceiver(liked.Token), liked.Token);

            _logger.LogInformation("Reading updates is done in background.");
            _rainbow.Shine(ShineCallback, ShineErrors, liked.Token);
        }

        /// <inheritdoc/>
        public void StartReaderOnly(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default)
            {
                _logger.LogInformation("Start's CancellationToken set to CancellationToken in UpdaterOptions");
                cancellationToken = _updaterOptions.CancellationToken;
            }

            _logger.LogWarning("Manual writing is enabled! You should write updates yourself.");

            _emergencyCancel = new CancellationTokenSource();
            using var liked = CancellationTokenSource.CreateLinkedTokenSource(
                _emergencyCancel.Token, cancellationToken);

            _logger.LogInformation("Blocking the current thread to read updates!");
            _rainbow.Shine(ShineCallback, ShineErrors, liked.Token);
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

        private Task ShineErrors(Exception exception, CancellationToken cancellationToken)
        {
            Logger.LogError(exception: exception, message: "Error in Rainbow!");
            return Task.CompletedTask;
        }

        private async Task ShineCallback(
            ShiningInfo<long, Update> shiningInfo, CancellationToken cancellationToken)
        {
            if (shiningInfo == null)
                return;

            if (_serviceDescriptors != null)
            {
                await ProcessUpdateFromServices(shiningInfo, cancellationToken);
            }
            else
            {
                await ProcessUpdate(shiningInfo, cancellationToken);
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
                try
                {
                    var updates = await _botClient.GetUpdatesAsync(offset, 100, timeOut,
                                                                    allowedUpdates: _updaterOptions.AllowedUpdates,
                                                                    cancellationToken: cancellationToken);

                    foreach (var update in updates)
                    {
                        await WriteAsync(update, cancellationToken);
                        offset = update.Id + 1;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogCritical(exception: e, "Auto update writer stopped due to an ecxeption.");
                    EmergencyCancel();
                    break;
                }
            }
        }

        private async Task ProcessUpdateFromServices(
            ShiningInfo<long, Update> shiningInfo, CancellationToken cancellationToken)
        {
            try
            {

                if (_serviceDescriptors == null)
                    throw new InvalidOperationException("Can't ProcessUpdateFromServices when there is no ServiceProvider.");

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var scopedHandlers = _scopedHandlerContainers
                    .Where(x => x.UpdateType == shiningInfo.Value.Type)
                    .Where(x => x.ShouldHandle(shiningInfo.Value));

                if (!scopedHandlers.Any())
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
                        if (!await HandleHandler(shiningInfo, handler, cancellationToken))
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
            ShiningInfo<long, Update> shiningInfo, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var singletonhandlers = _updateHandlers
                    .Where(x => x.UpdateType == shiningInfo.Value.Type)
                    .Where(x => x.ShouldHandle(shiningInfo.Value))
                    .Select(x => (IUpdateHandler)x);

                var scopedHandlers = _scopedHandlerContainers
                    .Where(x => x.UpdateType == shiningInfo.Value.Type)
                    .Where(x => x.ShouldHandle(shiningInfo.Value))
                    .Select(x => x.CreateInstance())
                    .Where(x => x != null)
                    .Cast<IScopedUpdateHandler>()
                    .Select(x => (IUpdateHandler)x);

                var handlers = singletonhandlers.Concat(scopedHandlers)
                    .OrderBy(x => x.Group);

                if (!(handlers.Any() || scopedHandlers.Any()))
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

                    if (!await HandleHandler(shiningInfo, handler, cancellationToken))
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
            ShiningInfo<long, Update> shiningInfo,
            IUpdateHandler handler,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            // Handle the shit.
            try
            {
                await handler.HandleAsync(this, shiningInfo);
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
                if (handler is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void EmergencyCancel()
        {
            _logger.LogWarning("Emergency cancel triggered.");
            _emergencyCancel?.Cancel();
        }
    }
}