using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.Poll"/>.
/// </summary>
public sealed class PollHandler : AnyHandler<Poll>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="PollHandler"/>.
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
    public PollHandler(Func<IContainer<Poll>, Task> callback,
                       IFilter<Poll>? filter,
                       int group)
        : base(UpdateType.Poll, x=> x.Poll, callback, filter, group)
    {
    }
}
