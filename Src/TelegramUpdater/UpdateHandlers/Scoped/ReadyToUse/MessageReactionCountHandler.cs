namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.MessageReactionCount"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MessageReactionCountHandler()
    : AnyHandler<MessageReactionCountUpdated>(x => x.MessageReactionCount)
{
    // Add any specific properties or methods for MessageReactionCountUpdated if needed.
}
