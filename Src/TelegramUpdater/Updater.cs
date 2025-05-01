using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot.Args;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater;

/// <summary>
/// The handler and some more info about it.
/// </summary>
/// <typeparam name="T">Type of the handler.</typeparam>
/// <param name="handler">The handler.</param>
/// <param name="group">Handling priority.</param>
public class HandlingInfo<T>(T handler, int group = 0)
{
    /// <summary>
    /// The handler.
    /// </summary>
    public T Handler { get; } = handler;

    /// <summary>
    /// Handling priority.
    /// </summary>
    public int Group { get; } = group;

    /// <summary>
    /// Swap this handle with something else, while keeping the extra as before.
    /// </summary>
    /// <typeparam name="Q"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public HandlingInfo<Q> SwapFace<Q>(Func<T, Q> func)
    {
        return new(func(Handler), Group);
    }
}

/// <summary>
/// Fetch updates from telegram and handle them.
/// </summary>
public sealed class Updater : IUpdater
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<HandlingInfo<ISingletonUpdateHandler>> _updateHandlers;
    private readonly List<HandlingInfo<IScopedUpdateHandlerContainer>> _scopedHandlerContainers;
    private readonly List<IExceptionHandler> _exceptionHandlers;
    private readonly ILogger<IUpdater> _logger;
    private readonly Type? _preUpdateProcessorType;
    private readonly MemoryCache _memoryCache;
    private UpdaterOptions _updaterOptions;
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
            
            var regex = new Regex(
                "^Too Many Requests: retry after (?<tryAfter>[0-9]*)$");
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
    public UpdateType[] AllowedUpdates => _updaterOptions.AllowedUpdates;

    /// <inheritdoc/>
    public IEnumerable<HandlingInfo<IScopedUpdateHandlerContainer>> ScopedHandlerContainers => _scopedHandlerContainers;

    /// <inheritdoc/>
    public IEnumerable<HandlingInfo<ISingletonUpdateHandler>> SingletonUpdateHandlers => _updateHandlers;

    /// <inheritdoc/>
    public object? this[object key]
    {
        get => _memoryCache.Get(key);
        set => AddItem(key, value, new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove,
        });
    }

    /// <inheritdoc/>
    public void EmergencyCancel()
    {
        _logger.LogWarning("Emergency cancel triggered.");
        _emergencyCancel?.Cancel();
    }

    /// <inheritdoc/>
    public bool ContainsKey(object key)
        => _memoryCache.TryGetValue(key, out var _);

    /// <inheritdoc/>
    public Updater AddSingletonUpdateHandler(
        ISingletonUpdateHandler updateHandler, int group = 0)
    {
        _updateHandlers.Add(new(updateHandler, group: group));
        _logger.LogInformation($"Added new singleton handler.");
        return this;
    }

    /// <inheritdoc/>
    public Updater AddScopedUpdateHandler(
        IScopedUpdateHandlerContainer scopedHandlerContainer, int group = 0)
    {
        _scopedHandlerContainers.Add(new(scopedHandlerContainer, group: group));
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
    public ValueTask WriteAsync(
        Update update, CancellationToken cancellationToken = default)
        => Rainbow.EnqueueAsync(update, cancellationToken);

    /// <inheritdoc/>
    public async Task StartAsync<TWriter>(
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

        if (_updaterOptions.AllowedUpdates == null)
        {
            // I need to recreate the options since it's readonly.
            _updaterOptions.AllowedUpdates = DetectAllowedUpdates();

            _logger.LogInformation(
                "Detected allowed updates automatically {allowed}",
                string.Join(", ", AllowedUpdates.Select(x => x.ToString()))
            );
        }

        // Link tokens. so we can use _emergencyCancel when required.
        _emergencyCancel = new CancellationTokenSource();
        using var liked = CancellationTokenSource.CreateLinkedTokenSource(
            _emergencyCancel.Token, cancellationToken);

        var writer = new TWriter();
        writer.SetUpdater(this);

        _logger.LogInformation(
            "Start reading updates from {writer}", typeof(TWriter));
        await writer.ExecuteAsync(liked.Token).ConfigureAwait(false);
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

    private UpdateType[] DetectAllowedUpdates()
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

    Func<IServiceScope?, ShiningInfo<long, Update>, CancellationToken, Task<bool>> GetHandlingJob(
        IScopedUpdateHandlerContainer container)
    {
        return async (scope, shiningInfo, cancellationToken) =>
        {
            var handler = container.CreateInstance(scope, logger: Logger);

            if (handler != null)
            {
                if (!await HandleHandler(
                    shiningInfo, handler, cancellationToken).ConfigureAwait(false))
                {
                    return true;
                }

            }

            return false;
        };
    }

    Func<IServiceScope?, ShiningInfo<long, Update>, CancellationToken, Task<bool>> GetHandlingJob(
        ISingletonUpdateHandler container)
    {
        return async (_, shiningInfo, cancellationToken) =>
        {
            if (!await HandleHandler(
                shiningInfo, container, cancellationToken).ConfigureAwait(false))
            {
                return true;
            }

            return false;
        };
    }

    private async Task ProcessUpdate(
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
                .Where(x => x.Handler.ShouldHandle(new(this, shiningInfo.Value)))
                .Select(x => x.SwapFace(x => GetHandlingJob(x)));

            var scopedHandlers = _scopedHandlerContainers
                .Where(x => x.Handler.ShouldHandle(new(this, shiningInfo.Value)))
                .Select(x => x.SwapFace(x => GetHandlingJob(x)));

            var handlers = singletonhandlers.Concat(scopedHandlers)
                .OrderBy(x => x.Group)
                .Select(x=> x.Handler);

            if (!handlers.Any()) return;

            // valid handlers for an update should process one by one
            // This change can be cut off when throwing an specified exception
            // Other exceptions are redirected to ExceptionHandler.
            foreach (var handler in handlers)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (await handler(serviceScope, shiningInfo, cancellationToken)
                    .ConfigureAwait(false))
                {
                    break;
                }
            }

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
            await handler.HandleAsync(this, shiningInfo).ConfigureAwait(false);
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
    public bool TryGetValue(object key, [NotNullWhen(true)] out object? value)
        => _memoryCache.TryGetValue(key, out value);

    /// <inheritdoc/>
    public void AddItem<T>(
        object key, T value, MemoryCacheEntryOptions? options = null)
    {
        _memoryCache.Set(key, value, options);
    }
}

