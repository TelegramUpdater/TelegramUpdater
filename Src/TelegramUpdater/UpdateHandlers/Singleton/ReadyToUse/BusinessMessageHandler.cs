using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
public sealed class BusinessMessageHandler(
    Func<MessageContainer, Task> callback,
    Filter<UpdaterFilterInputs<Message>>? filter = default)
    : AbstractMessageHandler(UpdateType.BusinessMessage, callback, filter, x => x.BusinessMessage)
{
}
