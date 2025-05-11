namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;
/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.BusinessConnection"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class BusinessConnectionControllerHandler()
    : DefaultControllerHandler<BusinessConnection>(x => x.BusinessConnection)
{
    // Add any specific properties or methods for BusinessConnection if needed.
}
