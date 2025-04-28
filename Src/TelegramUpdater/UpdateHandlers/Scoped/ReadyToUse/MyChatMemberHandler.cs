namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.MyChatMember"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MyChatMemberHandler()
    : AnyHandler<ChatMemberUpdated>(x => x.MyChatMember)
{
}
