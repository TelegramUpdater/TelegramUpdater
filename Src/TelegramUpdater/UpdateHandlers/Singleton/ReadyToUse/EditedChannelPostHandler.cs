using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
public sealed class EditedChannelPostHandler : AnyHandler<Message>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="EditedChannelPostHandler"/>.
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
    public EditedChannelPostHandler(Func<IContainer<Message>, Task> callback,
                                    IFilter<Message>? filter,
                                    int group)
        : base(UpdateType.EditedChannelPost, x=> x.EditedChannelPost, callback, filter, group)
    {
    }
}
