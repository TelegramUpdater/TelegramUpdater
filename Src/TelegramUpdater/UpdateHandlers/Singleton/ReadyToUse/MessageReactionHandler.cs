using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.MessageReaction"/>.
/// </summary>
public sealed class MessageReactionHandler(
    Func<IContainer<MessageReactionUpdated>, Task> callback,
    IFilter<UpdaterFilterInputs<MessageReactionUpdated>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<MessageReactionUpdated>(
        UpdateType.MessageReaction,
        callback,
        filter,
        x => x.MessageReaction,
        endpoint)
{
}
