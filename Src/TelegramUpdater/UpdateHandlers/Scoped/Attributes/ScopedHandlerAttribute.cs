namespace TelegramUpdater.UpdateHandlers.Scoped.Attributes;

/// <summary>
/// Extra info about an <see cref="IScopedUpdateHandler"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ScopedHandlerAttribute : Attribute
{
    /// <summary>
    /// Handling priority.
    /// </summary>
    public int Group { get; } = default;
}
