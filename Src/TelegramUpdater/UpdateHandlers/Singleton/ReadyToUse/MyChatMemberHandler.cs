using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.MyChatMember"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="MyChatMemberHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
public sealed class MyChatMemberHandler(
    Func<IContainer<ChatMemberUpdated>, Task> callback,
    IFilter<UpdaterFilterInputs<ChatMemberUpdated>>? filter) : AnyHandler<ChatMemberUpdated>(UpdateType.MyChatMember, x=> x.MyChatMember, callback, filter )
{
}
