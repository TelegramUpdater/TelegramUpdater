using Microsoft.Extensions.Logging;
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
    public UpdateType[] AllowedUpdates { get; }

    /// <summary>
    /// Enumerate over <see cref="IScopedUpdateHandlerContainer"/>s,
    /// registered to this instance
    /// of <see cref="IUpdater"/>.
    /// </summary>
    public IEnumerable<IScopedUpdateHandlerContainer> ScopedHandlerContainers { get; }

    /// <summary>
    /// Enumerate over <see cref="ISingletonUpdateHandler"/>s,
    /// registered to this instance
    /// of <see cref="IUpdater"/>.
    /// </summary>
    public IEnumerable<ISingletonUpdateHandler> SingletonUpdateHandlers { get; }

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
    Updater AddScopedUpdateHandler(IScopedUpdateHandlerContainer scopedHandlerContainer);

    /// <summary>
    /// Add your handler to this updater.
    /// </summary>
    /// <param name="updateHandler"></param>
    Updater AddSingletonUpdateHandler(ISingletonUpdateHandler updateHandler);

    /// <summary>
    /// Get current <see cref="TelegramBotClient"/>'s user information.
    /// </summary>
    /// <remarks>
    /// This method will cache! call freely.
    /// </remarks>
    Task<User> GetMeAsync();

    /// <summary>
    /// Manually write an update to the <see cref="Rainbow"/>
    /// </summary>
    ValueTask WriteAsync(Update update, CancellationToken cancellationToken = default);

    /// <summary>
    /// Use this to start writing updates
    /// ( using your custom writer <typeparamref name="TWriter"/> )
    /// to the updater. ( Blocking )
    /// </summary>
    /// <typeparam name="TWriter">
    /// Your custom update writer. a sub-class of
    /// <see cref="UpdateWriterAbs"/>.</typeparam>
    /// <param name="cancellationToken">To cancel the job manually,</param>
    public Task StartAsync<TWriter>(CancellationToken cancellationToken = default)
        where TWriter : UpdateWriterAbs, new();

    /// <summary>
    /// Use this to set or get extra data you may want to access everywhere
    /// <see cref="IUpdater"/> exists.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public object this[string key] { get; set; }

    /// <summary>
    /// Check if an <see cref="string"/> key exists in updater extra data.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key);
}
