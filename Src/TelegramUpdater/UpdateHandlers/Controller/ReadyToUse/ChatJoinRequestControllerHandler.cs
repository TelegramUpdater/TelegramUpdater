namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="ChatJoinRequest"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ChatJoinRequestControllerHandler()
    : DefaultControllerHandler<ChatJoinRequest>(x => x.ChatJoinRequest)
{
}
