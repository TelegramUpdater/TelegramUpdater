namespace TelegramUpdater.UpdateHandlers.Singleton;

/// <summary>
/// Interface for normal update handler ( known as singleton handlers )
/// </summary>
public interface ISingletonUpdateHandler : IUpdateHandler
{
    /// <summary>
    /// Type of update.
    /// </summary>
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Checks if an update can be handled in this
    /// <see cref="ISingletonUpdateHandler"/>.
    /// </summary>
    /// <returns></returns>
    public bool ShouldHandle(UpdaterFilterInputs<Update> inputs);
}
