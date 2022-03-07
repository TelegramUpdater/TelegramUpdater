using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
public sealed class PollAnswerHandler : AnyHandler<PollAnswer>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="PollAnswerHandler"/>.
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
    public PollAnswerHandler(Func<IContainer<PollAnswer>, Task> callback,
                             IFilter<PollAnswer>? filter,
                             int group)
        : base(UpdateType.PollAnswer, x=> x.PollAnswer, callback, filter, group)
    {
    }
}
