using Microsoft.Extensions.Logging;

namespace TelegramUpdater;

/// <summary>
/// Sets options for <see cref="IUpdater"/>.
/// </summary>
/// <remarks>
/// Sets options for <see cref="IUpdater"/>.
/// </remarks>
/// <param name="maxDegreeOfParallelism">
/// Maximum number of allowed concurrent update handling tasks.
/// </param>
/// <param name="logger">If you want to use your own logger.</param>
/// <param name="cancellationToken">
/// Default token to be used in Start method.
/// </param>
/// <param name="flushUpdatesQueue">Old updates will gone.</param>
/// <param name="allowedUpdates">Allowed updates.</param>
/// <param name="switchChatId">
/// By enabling this option, the updater will try to resolve
/// chat id from update and use it
/// as queue keys. if there's no user id available.
/// </param>
public readonly struct UpdaterOptions(
    int? maxDegreeOfParallelism = default,
    ILogger<IUpdater>? logger = default,
    bool flushUpdatesQueue = false,
    UpdateType[]? allowedUpdates = default,
    bool switchChatId = true,
    CancellationToken cancellationToken = default)
{

    /// <summary>
    /// Maximum number of allowed concurrent update handling tasks.
    /// </summary>
    public int? MaxDegreeOfParallelism { get; } = maxDegreeOfParallelism;

    /// <summary>
    /// If you want to use your own logger.
    /// </summary>
    public ILogger<IUpdater>? Logger { get; } = logger;

    /// <summary>
    /// Default token to be used in Start method.
    /// </summary>
    public CancellationToken CancellationToken { get; } = cancellationToken;

    /// <summary>
    /// Old updates will gone.
    /// </summary>
    public bool FlushUpdatesQueue { get; } = flushUpdatesQueue;

    /// <summary>
    /// Allowed updates.
    /// </summary>
    public UpdateType[] AllowedUpdates { get; } = allowedUpdates ?? [];

    /// <summary>
    /// By enabling this option, the updater will try to
    /// resolve chat id from update and use it
    /// as queue keys. if there's no user id available.
    /// </summary>
    /// <remarks>
    /// Normally, the updater tries to resolve a sender id ( user )
    /// from an update and use it as
    /// queue keys.
    /// </remarks>
    public bool SwitchChatId { get; } = switchChatId;
}
