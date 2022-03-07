using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.MyChatMember"/>.
/// </summary>
public sealed class MyChatMemberHandler : AnyHandler<ChatMemberUpdated>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="MyChatMemberHandler"/>.
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
    public MyChatMemberHandler(Func<IContainer<ChatMemberUpdated>, Task> callback,
                               IFilter<ChatMemberUpdated>? filter,
                               int group)
        : base(UpdateType.MyChatMember, x=> x.MyChatMember, callback, filter, group)
    {
    }
}
