namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class DeletedBusinessMessagesHandler()
    : DefaultHandler<BusinessMessagesDeleted>(x => x.DeletedBusinessMessages)
{
    // Add any specific properties or methods for BusinessMessagesDeleted if needed.
}
