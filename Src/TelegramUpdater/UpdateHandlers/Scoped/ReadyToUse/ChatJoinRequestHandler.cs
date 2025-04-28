namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="ChatJoinRequest"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class ChatJoinRequestHandler(int group = default)
    : AnyHandler<ChatJoinRequest>(x => x.ChatJoinRequest )
{
}
