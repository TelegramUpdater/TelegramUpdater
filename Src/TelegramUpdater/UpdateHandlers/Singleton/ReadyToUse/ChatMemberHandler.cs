using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChatMember"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="ChatMemberHandler"/>.
/// </remarks>
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
public sealed class ChatMemberHandler(
    Func<IContainer<ChatMemberUpdated>, Task> callback,
    IFilter<UpdaterFilterInputs<ChatMemberUpdated>>? filter,
    int group) : AnyHandler<ChatMemberUpdated>(UpdateType.ChatMember, x=> x.ChatMember, callback, filter, group)
{
}
