namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.BusinessConnection"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class BusinessConnectionHandler()
    : DefaultHandler<BusinessConnection>(x => x.BusinessConnection)
{
    // Add any specific properties or methods for BusinessConnection if needed.
}
