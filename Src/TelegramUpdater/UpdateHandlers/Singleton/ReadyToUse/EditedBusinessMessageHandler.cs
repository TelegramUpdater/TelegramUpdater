using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
public class EditedBusinessMessageHandler(
    Func<MessageContainer, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter = default,
    bool endpoint = true)
    : AbstractMessageHandler(
        UpdateType.EditedBusinessMessage,
        callback,
        filter,
        x => x.EditedBusinessMessage,
        endpoint)
{
}
