using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.MessageReaction"/>.
/// </summary>
public sealed class MessageReactionHandler(
    Func<IContainer<MessageReactionUpdated>, Task> callback,
    Filter<UpdaterFilterInputs<MessageReactionUpdated>>? filter = default)
    : DefaultHandler<MessageReactionUpdated>(UpdateType.MessageReaction, callback, filter, x => x.MessageReaction)
{
}
