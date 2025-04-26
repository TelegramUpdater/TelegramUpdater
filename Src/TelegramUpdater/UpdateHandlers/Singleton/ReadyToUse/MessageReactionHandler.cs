using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.MessageReaction"/>.
/// </summary>
public sealed class MessageReactionHandler(
    Func<IContainer<MessageReactionUpdated>, Task> callback,
    Filter<MessageReactionUpdated>? filter = default,
    int group = 0) : AnyHandler<MessageReactionUpdated>(UpdateType.MessageReaction, x => x.MessageReaction, callback, filter, group)
{
}
