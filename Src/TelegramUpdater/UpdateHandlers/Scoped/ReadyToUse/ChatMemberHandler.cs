namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChatMember"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ChatMemberHandler()
    : DefaultHandler<ChatMemberUpdated>(x => x.ChatMember)
{
}
