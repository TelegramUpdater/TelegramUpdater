namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChatMember"/>.
/// </summary>
public abstract class ChatMemberHandler : AnyHandler<ChatMemberUpdated>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected ChatMemberHandler(int group = default) : base(x => x.ChatMember, group)
    {
    }
}
