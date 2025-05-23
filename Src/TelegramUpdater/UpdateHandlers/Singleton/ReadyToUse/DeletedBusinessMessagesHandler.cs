using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
/// </summary>
public class DeletedBusinessMessagesHandler(
    Func<IContainer<BusinessMessagesDeleted>, Task> callback,
    IFilter<UpdaterFilterInputs<BusinessMessagesDeleted>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<BusinessMessagesDeleted>(
        UpdateType.DeletedBusinessMessages,
        callback,
        filter,
        x => x.DeletedBusinessMessages,
        endpoint)
{
}
