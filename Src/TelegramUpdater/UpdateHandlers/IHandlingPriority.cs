namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// Defines handling priority.
/// </summary>
public interface IHandlingPriority
{
    /// <summary>
    /// Handling priority, the lower the sooner to handle.
    /// </summary>
    int Priority { get; }
}
