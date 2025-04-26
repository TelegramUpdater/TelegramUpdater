namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.MessageReaction"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class MessageReactionHandler(int group = 0)
    : AnyHandler<MessageReactionUpdated>(x => x.MessageReaction, group)
{
    // Add any specific properties or methods for MessageReactionUpdated if needed.
}
