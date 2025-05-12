using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater;

/// <summary>
/// Base interface for updater.
/// </summary>
public interface IUpdater
{
    /// <summary>
    /// Bot client.
    /// </summary>
    ITelegramBotClient BotClient { get; }

    /// <summary>
    /// This updater logger.
    /// </summary>
    ILogger<IUpdater> Logger { get; }

    /// <summary>
    /// Options for this updater.
    /// </summary>
    UpdaterOptions UpdaterOptions { get; }

    /// <summary>
    /// Rainbow instance. responsible for parallel queuing.
    /// </summary>
    public Rainbow<long, Update> Rainbow { get; }

    /// <summary>
    /// A list of allowed updates. updater only receives
    /// and handles this kind of updates.
    /// </summary>
    public UpdateType[]? AllowedUpdates { get; }

    /// <summary>
    /// Detects allowed updates from handlers.
    /// </summary>
    /// <returns></returns>
    public UpdateType[] DetectAllowedUpdates();

    /// <summary>
    /// Enumerate over <see cref="IScopedUpdateHandlerContainer"/>s,
    /// registered to this instance
    /// of <see cref="IUpdater"/>.
    /// </summary>
    public IEnumerable<HandlingInfo<IScopedUpdateHandlerContainer>> ScopedHandlerContainers { get; }

    /// <summary>
    /// Enumerate over <see cref="ISingletonUpdateHandler"/>s,
    /// registered to this instance
    /// of <see cref="IUpdater"/>.
    /// </summary>
    public IEnumerable<HandlingInfo<ISingletonUpdateHandler>> SingletonUpdateHandlers { get; }

    /// <summary>
    /// Stop reader and writer ( if available ) tasks.
    /// </summary>
    public void EmergencyCancel();

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="exceptionHandler"></param>
    Updater AddExceptionHandler(IExceptionHandler exceptionHandler);

    /// <summary>
    /// Registers a scoped update handler with the updater. Scoped handlers are instantiated per update and can use dependency injection.
    /// </summary>
    /// <param name="updateHandler">
    /// The container describing the scoped handler. Use <see cref="ScopedUpdateHandlerContainerBuilder{THandler, TUpdate}"/> or extension methods like <see cref="ScopedUpdateHandlersExtensions.AddHandler"/> to create it.
    /// </param>
    /// <param name="options">
    /// Optional handling options such as group or layer. Controls handler priority and propagation.
    /// </param>
    /// <returns>The <see cref="Updater"/> instance for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register a scoped handler using the extension method (recommended)
    /// updater.AddHandler&lt;MyMessageHandler&gt;(
    ///     UpdateType.Message,
    ///     filter: ReadyFilters.OnCommand("hello"),
    ///     options: new HandlingOptions(group: 1));
    /// </code>
    /// <code>
    /// // Register a scoped handler manually
    /// var container = new ScopedUpdateHandlerContainerBuilder&lt;MyMessageHandler, Message&gt;(
    ///     UpdateType.Message,
    ///     filter: ReadyFilters.OnCommand("hello"));
    /// updater.AddScopedUpdateHandler(container);
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Prefer using the <c>AddHandler</c> extension methods for most scenarios:
    /// </para>
    /// <code>
    /// updater.AddHandler&lt;MyHandler&gt;(UpdateType.Message, filter: ReadyFilters.PM());
    /// </code>
    /// <para>
    /// This enables DI and filter support for your handler class.
    /// </para>
    /// </remarks>
    Updater AddScopedUpdateHandler(IScopedUpdateHandlerContainer updateHandler, HandlingOptions? options = default);

    /// <summary>
    /// Registers a singleton update handler with the updater. Singleton handlers are instantiated once and reused for all updates.
    /// </summary>
    /// <param name="updateHandler">
    /// The singleton handler instance implementing <see cref="ISingletonUpdateHandler"/>. You can use ready-to-use handlers or implement your own.
    /// </param>
    /// <param name="options">
    /// Optional handling options such as group or layer. Controls handler priority and propagation.
    /// </param>
    /// <returns>The <see cref="Updater"/> instance for chaining.</returns>
    /// <example>
    /// <code>
    /// // Register a custom singleton handler instance
    /// updater.AddSingletonUpdateHandler(new MyMessageHandler());
    /// </code>
    /// <code>
    /// // Register a ready-to-use handler with a filter
    /// updater.AddSingletonUpdateHandler(
    ///     new DefaultHandler&lt;Message&gt;(
    ///         updateType: UpdateType.Message,
    ///         callback: async container =&gt; await container.Response("Hello!"),
    ///         filter: ReadyFilters.OnCommand("hello")));
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Singleton handlers are best for stateless or lightweight logic. For handlers that require dependency injection or per-update state, use scoped handlers via <see cref="AddHandler"/> or <see cref="AddScopedUpdateHandler"/>.
    /// </para>
    /// <para>
    /// You can also use extension methods like <c>Handle</c> for minimal singleton handler registration:
    /// </para>
    /// <code>
    /// updater.Handle(
    ///     UpdateType.Message,
    ///     async (MessageContainer container) =&gt; await container.Response("Hi!"),
    ///     ReadyFilters.OnCommand("hi"));
    /// </code>
    /// </remarks>
    Updater AddSingletonUpdateHandler(ISingletonUpdateHandler updateHandler, HandlingOptions? options = default);

    /// <summary>
    /// Get current <see cref="TelegramBotClient"/>'s user information.
    /// </summary>
    /// <remarks>
    /// This method will cache! call freely.
    /// </remarks>
    Task<User> GetMe();

    /// <summary>
    /// Manually write an update to the <see cref="Rainbow"/>
    /// </summary>
    ValueTask Write(Update update, CancellationToken cancellationToken = default);

    /// <summary>
    /// Use this to start writing updates
    /// (using your custom writer <typeparamref name="TWriter"/>)
    /// to the updater.
    /// </summary>
    /// <typeparam name="TWriter">
    /// Your custom update writer. a sub-class of
    /// <see cref="AbstractUpdateWriter"/>.</typeparam>
    /// <param name="cancellationToken">To cancel the job manually,</param>
    public Task Start<TWriter>(CancellationToken cancellationToken = default)
        where TWriter : AbstractUpdateWriter, new();

    /// <summary>
    /// Use this to set or get extra data you may want to access everywhere
    /// <see cref="IUpdater"/> exists.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public object? this[string key] { get; set; }

    /// <summary>
    /// Adds an item to updater's storage.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public void SetItem<T>(string key, T value, MemoryCacheEntryOptions? options = default);

    /// <summary>
    /// Remove item from updater's storage
    /// </summary>
    /// <param name="key"></param>
    public void RemoveItem(string key);

    /// <summary>
    /// Check if an <see cref="string"/> key exists in updater extra data.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key);

    /// <summary>
    /// Tries to take a value out of this.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue<TValue>(string key, [NotNullWhen(true)] out TValue? value);

    /// <summary>
    /// The memory cache associated with the <see cref="IUpdater"/>.
    /// </summary>
    public IMemoryCache MemoryCache { get; }
}
