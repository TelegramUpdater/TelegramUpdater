namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="ChatJoinRequest"/>.
/// </summary>
public abstract class ChatJoinRequestHandler : AnyHandler<ChatJoinRequest>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected ChatJoinRequestHandler(int group = default)
        : base(x => x.ChatJoinRequest, group)
    {
    }
}
