using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
public sealed class ChosenInlineResultHandler : AnyHandler<ChosenInlineResult>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="ChosenInlineResultHandler"/>.
    /// </summary>
    /// <param name="callback">
    /// A callback function that will be called when an <see cref="Update"/>
    /// passes the <paramref name="filter"/>.
    /// </param>
    /// <param name="filter">
    /// A filter to choose the right update to be handled inside
    /// <paramref name="callback"/>.
    /// </param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public ChosenInlineResultHandler(
        Func<IContainer<ChosenInlineResult>, Task> callback,
        IFilter<ChosenInlineResult>? filter = default,
        int group = default)
        : base(UpdateType.ChosenInlineResult,
              x => x.ChosenInlineResult,
              callback,
              filter,
              group)
    {
    }
}
