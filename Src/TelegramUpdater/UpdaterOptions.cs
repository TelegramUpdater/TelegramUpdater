using Microsoft.Extensions.Logging;

namespace TelegramUpdater;

/// <summary>
/// Sets options for <see cref="IUpdater"/>.
/// </summary>
public readonly struct UpdaterOptions
{
    /// <summary>
    /// Sets options for <see cref="IUpdater"/>.
    /// </summary>
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
    public UpdaterOptions(
        int? maxDegreeOfParallelism = default,
        ILogger<IUpdater>? logger = default,
        CancellationToken cancellationToken = default,
        bool flushUpdatesQueue = false,
        UpdateType[]? allowedUpdates = default,
        bool switchChatId = true)
    {
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        Logger = logger;
        CancellationToken = cancellationToken;
        FlushUpdatesQueue = flushUpdatesQueue;
        AllowedUpdates = allowedUpdates ?? Array.Empty<UpdateType>();
        SwitchChatId = switchChatId;
    }

    /// <summary>
    /// Maximum number of allowed concurrent update handling tasks.
    /// </summary>
    public int? MaxDegreeOfParallelism { get; }

    /// <summary>
    /// If you want to use your own logger.
    /// </summary>
    public ILogger<IUpdater>? Logger { get; }

    /// <summary>
    /// Default token to be used in Start method.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Old updates will gone.
    /// </summary>
    public bool FlushUpdatesQueue { get; }

    /// <summary>
    /// Allowed updates.
    /// </summary>
    public UpdateType[] AllowedUpdates { get; }

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
    public bool SwitchChatId { get; } = false;
}
