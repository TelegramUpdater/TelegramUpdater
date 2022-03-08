namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.MyChatMember"/>.
/// </summary>
public abstract class MyChatMemberHandler : AnyHandler<ChatMemberUpdated>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected MyChatMemberHandler(int group = default)
        : base(x => x.MyChatMember, group)
    {
    }
}
