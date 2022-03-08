using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChannelPost"/>.
/// </summary>
public sealed class ChannelPostHandler : AnyHandler<Message>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="ChannelPostHandler"/>.
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
    public ChannelPostHandler(Func<IContainer<Message>, Task> callback,
                              IFilter<Message>? filter,
                              int group) 
        : base(UpdateType.ChannelPost, x=> x.ChannelPost, callback, filter, group)
    {
    }
}
