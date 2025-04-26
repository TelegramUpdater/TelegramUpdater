namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.MessageReactionCount"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class MessageReactionCountHandler(int group = 0)
    : AnyHandler<MessageReactionCountUpdated>(x => x.MessageReactionCount, group)
{
    // Add any specific properties or methods for MessageReactionCountUpdated if needed.
}
