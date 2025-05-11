namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped update controller for <see cref="UpdateType.DeletedBusinessMessages"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class DeletedBusinessMessagesControllerHandler()
    : DefaultControllerHandler<BusinessMessagesDeleted>(x => x.DeletedBusinessMessages)
{
    // Add any specific properties or methods for BusinessMessagesDeleted if needed.
}
