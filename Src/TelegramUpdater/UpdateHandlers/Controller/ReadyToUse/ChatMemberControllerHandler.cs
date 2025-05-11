namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.ChatMember"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ChatMemberControllerHandler()
    : DefaultControllerHandler<ChatMemberUpdated>(x => x.ChatMember)
{
}
