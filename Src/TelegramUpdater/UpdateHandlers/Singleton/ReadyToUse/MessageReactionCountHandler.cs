using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.MessageReactionCount"/>.
/// </summary>
public class MessageReactionCountHandler(
    Func<IContainer<MessageReactionCountUpdated>, Task> callback,
    IFilter<UpdaterFilterInputs<MessageReactionCountUpdated>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<MessageReactionCountUpdated>(
        UpdateType.MessageReactionCount,
        callback,
        filter,
        x => x.MessageReactionCount,
        endpoint)
{
}
