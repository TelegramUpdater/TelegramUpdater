namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.MessageReaction"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MessageReactionHandler()
    : AnyHandler<MessageReactionUpdated>(x => x.MessageReaction)
{
    // Add any specific properties or methods for MessageReactionUpdated if needed.
}
