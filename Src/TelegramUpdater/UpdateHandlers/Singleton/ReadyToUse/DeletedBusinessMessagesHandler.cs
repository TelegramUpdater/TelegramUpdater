using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
/// </summary>
public sealed class DeletedBusinessMessagesHandler(
    Func<IContainer<BusinessMessagesDeleted>, Task> callback,
    Filter<UpdaterFilterInputs<BusinessMessagesDeleted>>? filter = default,
    int group = 0) : AnyHandler<BusinessMessagesDeleted>(UpdateType.DeletedBusinessMessages, x => x.DeletedBusinessMessages, callback, filter, group)
{
}
