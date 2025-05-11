namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.MyChatMember"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MyChatMemberControllerHandler()
    : DefaultControllerHandler<ChatMemberUpdated>(x => x.MyChatMember)
{
}
