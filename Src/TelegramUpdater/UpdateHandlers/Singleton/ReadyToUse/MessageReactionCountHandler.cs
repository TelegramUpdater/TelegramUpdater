using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.MessageReactionCount"/>.
/// </summary>
public sealed class MessageReactionCountHandler(
    Func<IContainer<MessageReactionCountUpdated>, Task> callback,
    Filter<MessageReactionCountUpdated>? filter = default,
    int group = 0) : AnyHandler<MessageReactionCountUpdated>(UpdateType.MessageReactionCount, x => x.MessageReactionCount, callback, filter, group)
{
}
