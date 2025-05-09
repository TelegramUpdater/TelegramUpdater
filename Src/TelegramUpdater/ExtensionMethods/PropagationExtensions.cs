namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// Extensions to manage propagation inside handlers.
/// </summary>
public static class PropagationExtensions
{
    /// <summary>
    /// All pending handlers for this update will be ignored after throwing this.
    /// </summary>
    internal static void StopPropagation<T>(this IContainer<T> _)
        where T : class => throw new StopPropagationException();

    /// <summary>
    /// Continue to the next pending handler for this update and ignore the rest of this handler.
    /// </summary>
    internal static void ContinuePropagation<T>(this IContainer<T> _)
        where T : class => throw new ContinuePropagationException();
}
