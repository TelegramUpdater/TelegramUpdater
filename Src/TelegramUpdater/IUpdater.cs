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
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <param name="scopedHandlerContainer">
    /// Use <see cref="ScopedUpdateHandlerContainerBuilder{THandler, TUpdate}"/>
    /// To Create a new <see cref="IScopedUpdateHandlerContainer"/>
    /// </param>
    /// <param name="options">Information about how a handler should be handled.</param>
    Updater AddHandler(IScopedUpdateHandlerContainer scopedHandlerContainer, HandlingOptions? options = default);

    /// <summary>
    /// Add your handler to this updater.
    /// </summary>
    /// <param name="updateHandler"></param>
    /// <param name="options">Information about how a handler should be handled.</param>
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
