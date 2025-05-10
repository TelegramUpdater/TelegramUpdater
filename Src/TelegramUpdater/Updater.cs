using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot.Args;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.Helpers;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater;

/// <summary>
/// Fetch updates from telegram and handle them.
/// </summary>
public sealed partial class Updater : IUpdater
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<HandlingInfo<ISingletonUpdateHandler>> _updateHandlers;
    private readonly List<HandlingInfo<IScopedUpdateHandlerContainer>> _scopedHandlerContainers;
    private readonly List<IExceptionHandler> _exceptionHandlers;
    private readonly ILogger<IUpdater> _logger;
    private readonly Type? _preUpdateProcessorType;
    private readonly MemoryCache _memoryCache;
    private readonly UpdaterOptions _updaterOptions;
    private User? _me = null;

    // Updater can use this to cancel update processing when it's needed.
    private CancellationTokenSource? _emergencyCancel;

    // Updater can use this to change the behavior on scoped handlers.
    // If it's present, then DI will be available inside scoped handlers
    private readonly IServiceScopeFactory? _scopeFactory;

    // This the main class responsible for queuing updates
    // It handles everything related to process priority and more
    private readonly Rainbow<long, Update> _rainbow;

    /// <summary>
    /// Creates an instance of updater to fetch updates from
    /// telegram and handle them.
    /// </summary>
    /// <param name="botClient">Telegram bot client</param>
    /// <param name="updaterOptions">Options for this updater.</param>
    /// <param name="scopeFactory">
    /// Optional service provider.
    /// </param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="scopeFactory"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="customKeyResolver">
    /// If you wanna customize the way updater resolves a sender id from
    /// <see cref="Update"/> 
    /// ( as queue keys ), you can pass your own. <b>Use with care!</b>
    /// </param>
    /// <param name="outgoingRateControl">
    /// <b>[BETA]</b> - Applies a wait if you cross the telegram limits border.
    /// <c>"Too Many Requests: retry after xxx"</c> Error.
    /// </param>
    public Updater(
        ITelegramBotClient botClient,
        UpdaterOptions? updaterOptions = default,
        IServiceScopeFactory? scopeFactory = default,
        Type? preUpdateProcessorType = default,
        Func<Update, long>? customKeyResolver = default,
        bool outgoingRateControl = false)
    {
        _botClient = botClient ??
            throw new ArgumentNullException(nameof(botClient));
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        if (outgoingRateControl)
            _botClient.OnApiResponseReceived += OnApiResponseReceived;
        _updaterOptions = updaterOptions?? new UpdaterOptions();
        _preUpdateProcessorType = preUpdateProcessorType;

        if (_preUpdateProcessorType is not null)
        {
            if (!typeof(AbstractPreUpdateProcessor)
                .IsAssignableFrom(_preUpdateProcessorType))
            {
                throw new InvalidOperationException(
                    $"Input type for preUpdateProcessorType " +
                    "( {preUpdateProcessorType} ) should be an" +
                    " instance of AbstractPreUpdateProcessor.");
            }

            if (scopeFactory is null)
            {
                if (_preUpdateProcessorType
                    .GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new InvalidOperationException(
                        $"Input type for preUpdateProcessorType " +
                        "( {preUpdateProcessorType} ) should have" +
                        " an empty ctor when there's no service provider.");
                }
            }
        }

        _scopeFactory = scopeFactory;
        _updateHandlers = [];
        _exceptionHandlers = [];
        _scopedHandlerContainers = [];
        _rainbow = new Rainbow<long, Update>(
            _updaterOptions.MaxDegreeOfParallelism ??
                Environment.ProcessorCount,
            customKeyResolver ?? QueueKeyResolver,
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

    private async ValueTask OnApiResponseReceived(
        ITelegramBotClient botClient,
        ApiResponseEventArgs args,
        CancellationToken cancellationToken = default)
    {
        if (args.ResponseMessage.StatusCode == HttpStatusCode.TooManyRequests)
        {
#if NET8_0_OR_GREATER
            var failedApiResponseMessage = await args.ResponseMessage.Content
                .ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
            var failedApiResponseMessage = await args.ResponseMessage.Content
                .ReadAsStreamAsync().ConfigureAwait(false);
#endif
            var jsonObject = await JsonDocument.ParseAsync(
                failedApiResponseMessage, cancellationToken: cancellationToken).ConfigureAwait(false);

            var description = jsonObject.RootElement.GetProperty("description")
                .GetString();

            if (description is null) return;

#if NET8_0_OR_GREATER
            var regex = ToManyRequestsRegex();
#else
            var regex = new Regex(
                "^Too Many Requests: retry after (?<tryAfter>[0-9]*)$",
                RegexOptions.None,
                TimeSpan.FromSeconds(5));
#endif
            Match match = regex.Match(description);
            if (match.Success)
            {
                var tryAfterSeconds = int.Parse(
                    match.Groups["tryAfter"].Value, CultureInfo.InvariantCulture);

                Logger.LogWarning("A wait of {seconds} is required! caused by {method}",
                    tryAfterSeconds, args.ApiRequestEventArgs.Request.MethodName);
                await Task.Delay(tryAfterSeconds * 1000, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Creates an instance of updater to fetch updates from
    /// telegram and handle them.
    /// </summary>
    /// <param name="updaterOptions">Options for this updater.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="customKeyResolver">
    /// If you wanna customize the way updater resolves a sender id
    /// from <see cref="Update"/> 
    /// ( as queue keys ), you can pass your own. <b>Use with care!</b>
    /// </param>
    /// <param name="outgoingRateControl">
    /// <b>[BETA]</b> - Applies a wait if you cross the telegram limits border.
    /// <c>"Too Many Requests: retry after xxx"</c> Error.
    /// </param>
    public Updater(
        UpdaterOptions? updaterOptions = default,
        Type? preUpdateProcessorType = default,
        Func<Update, long>? customKeyResolver = default,
        bool outgoingRateControl = default): this(
            botClient: new TelegramBotClient(
                updaterOptions?.BotToken?? throw new ArgumentNullException(
                    nameof(updaterOptions), "Bot token in updater options is null.")),
            updaterOptions: updaterOptions,
            preUpdateProcessorType: preUpdateProcessorType,
            customKeyResolver: customKeyResolver,
            outgoingRateControl: outgoingRateControl)
    { }

    /// <summary>
    /// Creates an instance of updater to fetch updates from
    /// telegram and handle them.
    /// </summary>
    /// <param name="botToken">Your telegram bot token. This will replace <see cref="UpdaterOptions.BotToken"/></param>
    /// <param name="updaterOptions">Options for this updater.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="customKeyResolver">
    /// If you wanna customize the way updater resolves a sender id
    /// from <see cref="Update"/> 
    /// ( as queue keys ), you can pass your own. <b>Use with care!</b>
    /// </param>
    /// <param name="outgoingRateControl">
    /// <b>[BETA]</b> - Applies a wait if you cross the telegram limits border.
    /// <c>"Too Many Requests: retry after xxx"</c> Error.
    /// </param>
    public Updater(
        string botToken,
        UpdaterOptions? updaterOptions = default,
        Type? preUpdateProcessorType = default,
        Func<Update, long>? customKeyResolver = default,
        bool outgoingRateControl = default): this(
            updaterOptions: UpdaterExtensions.RedesignOptions(
                updaterOptions: updaterOptions, newBotToken: botToken),
            preUpdateProcessorType: preUpdateProcessorType,
            customKeyResolver: customKeyResolver,
            outgoingRateControl: outgoingRateControl)
    { }

    /// <inheritdoc/>
    public UpdaterOptions UpdaterOptions => _updaterOptions;

    /// <inheritdoc/>
    public ITelegramBotClient BotClient => _botClient;

    /// <inheritdoc/>
    public ILogger<IUpdater> Logger => _logger;

    /// <inheritdoc/>
    public Rainbow<long, Update> Rainbow => _rainbow;

    /// <inheritdoc/>
    public UpdateType[]? AllowedUpdates => _updaterOptions.AllowedUpdates;

    /// <inheritdoc/>
    public IEnumerable<HandlingInfo<IScopedUpdateHandlerContainer>> ScopedHandlerContainers => _scopedHandlerContainers;

    /// <inheritdoc/>
    public IEnumerable<HandlingInfo<ISingletonUpdateHandler>> SingletonUpdateHandlers => _updateHandlers;

    /// <inheritdoc/>
    public void EmergencyCancel()
    {
        _logger.LogWarning("Emergency cancel triggered.");
        _emergencyCancel?.Cancel();
    }

    /// <inheritdoc/>
    public Updater AddSingletonUpdateHandler(
        ISingletonUpdateHandler singletonUpdateHandler,
        HandlingOptions? options = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(singletonUpdateHandler);
#else
        if (singletonUpdateHandler is null)
        {
            throw new ArgumentNullException(nameof(singletonUpdateHandler));
        }
#endif

        options ??= singletonUpdateHandler
            .GetHandlingOptionsFromAttibute();

        _updateHandlers.Add(new(singletonUpdateHandler, options: options));
        _logger.LogInformation($"Added new singleton handler.");
        return this;
    }

    /// <inheritdoc/>
    public Updater AddScopedUpdateHandler(
        IScopedUpdateHandlerContainer scopedHandlerContainer,
        HandlingOptions? options = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(scopedHandlerContainer);
#else
        if (scopedHandlerContainer is null)
        {
            throw new ArgumentNullException(nameof(scopedHandlerContainer));
        }
#endif

        options ??= scopedHandlerContainer
            .GetHandlingOptionsFromAttibute();

        _scopedHandlerContainers.Add(new(scopedHandlerContainer, options: options));
        _logger.LogInformation("Added new scoped handler {handler}",
            scopedHandlerContainer.ScopedHandlerType);
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
    public ValueTask Write(
        Update update, CancellationToken cancellationToken = default)
        => Rainbow.EnqueueAsync(update, cancellationToken);

    /// <inheritdoc/>
    public async Task Start<TWriter>(
        CancellationToken cancellationToken = default)
        where TWriter : AbstractUpdateWriter, new()
    {
        if (cancellationToken == default)
        {
            _logger.LogInformation(
                "Start's CancellationToken set to " +
                "CancellationToken in UpdaterOptions");
            cancellationToken = _updaterOptions.CancellationToken;
        }

        // Link tokens. so we can use _emergencyCancel when required.
        _emergencyCancel = new CancellationTokenSource();
        using var liked = CancellationTokenSource.CreateLinkedTokenSource(
            _emergencyCancel.Token, cancellationToken);

        var writer = new TWriter();
        writer.SetUpdater(this);

        _logger.LogInformation(
            "Start reading updates from {writer}", typeof(TWriter));
        await writer.Run(liked.Token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<User> GetMe()
    {
        _me ??= await _botClient.GetMe(cancellationToken: UpdaterOptions.CancellationToken)
            .ConfigureAwait(false);

        return _me;
    }

    private long QueueKeyResolver(Update update)
    {
        var userId = update.GetSenderId();

        if (userId is null)
        {
            if (UpdaterOptions.SwitchChatId)
            {
                var chatId = update.GetChatId();
                if (chatId is null) return 0;
                return chatId.Value;
            }
            return 0;
        }
        return userId.Value;
    }

    /// <inheritdoc />
    public UpdateType[] DetectAllowedUpdates()
        => _updateHandlers
            .Select(x => x.Handler.UpdateType)
            .Concat(_scopedHandlerContainers.Select(x => x.Handler.UpdateType))
            .Distinct()
            .ToArray();

    private Task ShineErrors(
        Exception exception, CancellationToken cancellationToken)
    {
        Logger.LogError(exception: exception, message: "Error in Rainbow!");
        return Task.CompletedTask;
    }

    private async Task ShineCallback(
        ShiningInfo<long, Update> shiningInfo,
        CancellationToken cancellationToken)
    {
        if (shiningInfo == null)
            return;

        var servicesAvailabe = _scopeFactory is not null;

        if (_preUpdateProcessorType != null)
        {
            AbstractPreUpdateProcessor processor;
            if (servicesAvailabe)
            {
                var scope = _scopeFactory!.CreateAsyncScope();
                await using (scope.ConfigureAwait(false))
                {
                    processor = (AbstractPreUpdateProcessor)scope
                    .ServiceProvider
                    .GetRequiredService(_preUpdateProcessorType);
                }
            }
            else
            {
                processor = (AbstractPreUpdateProcessor)Activator
                    .CreateInstance(_preUpdateProcessorType)!;
                processor.SetUpdater(this);
            }

            if (!await processor.PreProcessor(shiningInfo).ConfigureAwait(false))
            {
                return;
            }
        }

        await ProcessUpdate(shiningInfo, cancellationToken).ConfigureAwait(false);
    }

    Func<IServiceScope?, HandlerInput, CancellationToken, Task<bool>> GetHandlingJob(
        IScopedUpdateHandlerContainer container)
    {
        return async (scope, input, cancellationToken) =>
        {
            var handler = container.CreateInstance(scope, logger: Logger);

            if (handler != null)
            {
                if (!await HandleHandler(
                    handler: handler,
                    input: input,
                    cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    // Means stop propagation
                    return true;
                }

                return handler.Endpoint;
            }

            return false;
        };
    }

    Func<IServiceScope?, HandlerInput, CancellationToken, Task<bool>> GetHandlingJob(
        ISingletonUpdateHandler handler)
    {
        return async (_, input, cancellationToken) =>
        {
            if (!await HandleHandler(
                handler: handler,
                input: input,
                cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                // Means stop propagation
                return true;
            }

            return handler.Endpoint;
        };
    }

    class HandlingJobInfo(
        HandlingOptions options,
        Func<IServiceScope?, HandlerInput, CancellationToken, Task<bool>> handler,
        Func<UpdaterFilterInputs<Update>, bool> filter)
    {
        public HandlingOptions Options { get; } = options;

        public Func<IServiceScope?, HandlerInput, CancellationToken, Task<bool>> Handler { get; } = handler;

        public Func<UpdaterFilterInputs<Update>, bool> Filter { get; } = filter;
    }

    // I know
#pragma warning disable MA0051 // Method is too long
    private async Task ProcessUpdate(
#pragma warning restore MA0051 // Method is too long
        ShiningInfo<long, Update> shiningInfo,
        CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            AsyncServiceScope? serviceScope = default;
            if (_scopeFactory is not null)
                // The scope is created at beginning of processing pipeline for an update.
                // The update may trigger more than one handler, so the scope is persisting between them
                serviceScope = _scopeFactory.CreateAsyncScope();

            var singletonhandlers = _updateHandlers
                .Select(x => new HandlingJobInfo(
                    HandlingOptions.OrDefault(x.Options),
                    GetHandlingJob(x.Handler),
                    x.Handler.ShouldHandle));

            var scopedHandlers = _scopedHandlerContainers
                .Select(x => new HandlingJobInfo(
                    HandlingOptions.OrDefault(x.Options),
                    GetHandlingJob(x.Handler),
                    x.Handler.ShouldHandle));

            var handlers = singletonhandlers.Concat(scopedHandlers);

            if (!handlers.Any()) return;

            var scopeId = Guid.NewGuid();
            var wrapedScopedId = new HandlingStoragesKeys.ScopeId(scopeId);
            var scopeCts = new CancellationTokenSource();
            var scopeEndedToken = new CancellationChangeToken(scopeCts.Token);

            // Group handler by layer id
            var layerd = handlers.GroupBy(x => x.Options.LayerInfo.Key);

            // The otter loop is over separate layers, so breaking inner loop won't effect this
            foreach (var layer in layerd.OrderBy(x=> x.Key))
            {
                var layerId = new HandlingStoragesKeys.LayerId(scopeId, layer.Key);
                var layerCts = new CancellationTokenSource();
                var layerEndedToken = new CancellationChangeToken(layerCts.Token);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // valid handlers for an update should process one by one
                // This change can be cut off when throwing an specified exception
                // Other exceptions are redirected to ExceptionHandler.
                foreach (var (handlingInfo, indexInLayer) in layer
                    .OrderBy(x => x.Options.Group)
                    .Select((x, i) => (x, i)))
                {
                    //var groupId = new HandlingStoragesKeys.GroupId(scopeId, layer.Key, handlingInfo.Options.Group);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var layerInfo = handlingInfo.Options.LayerInfo;
                    var groupIndex = handlingInfo.Options.Group;

                    if (!handlingInfo.Filter(
                        new UpdaterFilterInputs<Update>(this, shiningInfo.Value, scopeId, layerInfo, groupIndex, indexInLayer)))
                        // Filter didn't pass, ignore
                        continue;

                    if (await handlingInfo.Handler(
                        serviceScope,
                        new HandlerInput(
                            updater: this,
                            shiningInfo: shiningInfo,
                            scopeId: scopeId,
                            layerInfo: layerInfo,
                            group: groupIndex,
                            index: indexInLayer,
                            scopeChangeToken: scopeEndedToken,
                            layerChangeToken: layerEndedToken),
                        cancellationToken)
                        .ConfigureAwait(false))
                    {
                        // Propagation stop only effects handler in the same layer.
                        break;
                    }
                }

                // End of the layer
#if NET8_0_OR_GREATER
                await layerCts.CancelAsync().ConfigureAwait(false);
#else
                layerCts.Cancel();
#endif
            }

            // End of the scope
#if NET8_0_OR_GREATER
            await scopeCts.CancelAsync().ConfigureAwait(false);
#else
            scopeCts.Cancel();
#endif
            if (serviceScope is not null)
                await serviceScope.Value.DisposeAsync().ConfigureAwait(false);
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
        IUpdateHandler handler,
        HandlerInput input,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        // Handle the shit.
        try
        {
            await handler.HandleAsync(input).ConfigureAwait(false);
        }
        // Cut handlers chain.
        catch (StopPropagationException)
        {
            return false;
        }
        catch (ContinuePropagationException)
        {
            return true;
        }
        catch (Exception ex)
        {
            // Do exception handlers
            var exHandlers = _exceptionHandlers
                .Where(x => x.TypeIsMatched(ex.GetType()) && x.IsAllowedHandler(handler.GetType()) && x.MessageMatched(ex.Message));

            foreach (var exHandler in exHandlers)
            {
                await exHandler.Callback(this, ex).ConfigureAwait(false);
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
    public IMemoryCache MemoryCache => _memoryCache;

    /// <inheritdoc/>
    public object? this[string key]
    {
        get => _memoryCache.Get(key);
        set => SetItem(key, value, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove,
        });
    }

    /// <inheritdoc/>
    public bool TryGetValue<TValue>(string key, [NotNullWhen(true)] out TValue? value)
    {
        var result = _memoryCache.TryGetValue(key, out value);

        if (!result)
        {
            Logger.LogDebug("Key: {key} not found", key);
            Logger.LogDebug("Available keys:\n- {keys}", string.Join("\n- ", _memoryCache.Keys));
        }

        return result;
    }

    /// <inheritdoc/>
    public void SetItem<T>(
        string key, T value, MemoryCacheEntryOptions? options = null)
    {
        _memoryCache.Set(key, value, options);
    }

    /// <inheritdoc/>
    public void RemoveItem(string key)
    {
        _memoryCache.Remove(key);
    }

    /// <inheritdoc/>
    public bool ContainsKey(string key)
        => _memoryCache.TryGetValue(key, out var _);

#if NET8_0_OR_GREATER
    [GeneratedRegex("^Too Many Requests: retry after (?<tryAfter>[0-9]*)$", RegexOptions.None, matchTimeoutMilliseconds: 5000)]
    private static partial Regex ToManyRequestsRegex();
#endif
}

