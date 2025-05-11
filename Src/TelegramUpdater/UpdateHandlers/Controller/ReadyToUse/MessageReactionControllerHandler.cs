namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.MessageReaction"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MessageReactionControllerHandler()
    : DefaultControllerHandler<MessageReactionUpdated>(x => x.MessageReaction)
{
    // Add any specific properties or methods for MessageReactionUpdated if needed.
}
