using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
public class BusinessMessageHandler(
    Func<MessageContainer, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter = default,
    bool endpoint = true)
    : AbstractMessageHandler(
        UpdateType.BusinessMessage,
        callback,
        filter,
        x => x.BusinessMessage,
        endpoint)
{
}
