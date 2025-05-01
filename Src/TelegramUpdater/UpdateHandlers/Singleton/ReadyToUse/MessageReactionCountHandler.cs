using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.MessageReactionCount"/>.
/// </summary>
public sealed class MessageReactionCountHandler(
    Func<IContainer<MessageReactionCountUpdated>, Task> callback,
    Filter<UpdaterFilterInputs<MessageReactionCountUpdated>>? filter = default)
    : AnyHandler<MessageReactionCountUpdated>(UpdateType.MessageReactionCount, callback, filter, x => x.MessageReactionCount)
{
}
