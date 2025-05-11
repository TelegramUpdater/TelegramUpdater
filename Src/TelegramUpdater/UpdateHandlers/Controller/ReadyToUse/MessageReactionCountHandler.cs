namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.MessageReactionCount"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MessageReactionCountControllerHandler()
    : DefaultControllerHandler<MessageReactionCountUpdated>(x => x.MessageReactionCount)
{
    // Add any specific properties or methods for MessageReactionCountUpdated if needed.
}
