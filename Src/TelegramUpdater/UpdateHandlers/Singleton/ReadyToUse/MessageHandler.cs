using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.Message"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="MessageHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
/// <param name="endpoint">Determines if this is and endpoint handler.</param>
public class MessageHandler(
    Func<MessageContainer, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter = default,
    bool endpoint = true)
    : AbstractMessageHandler(
        updateType: UpdateType.Message,
        callback: callback,
        filter: filter,
        getT: x => x.Message,
        endpoint: endpoint)
{
}
