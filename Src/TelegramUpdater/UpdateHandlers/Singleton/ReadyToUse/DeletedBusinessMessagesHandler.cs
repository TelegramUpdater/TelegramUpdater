using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
/// </summary>
public sealed class DeletedBusinessMessagesHandler : AnyHandler<BusinessMessagesDeleted>
{
    public DeletedBusinessMessagesHandler(
        Func<IContainer<BusinessMessagesDeleted>, Task> callback,
        Filter<BusinessMessagesDeleted>? filter = default,
        int group = 0)
        : base(UpdateType.DeletedBusinessMessages, x => x.DeletedBusinessMessages, callback, filter, group)
    {
    }
}
