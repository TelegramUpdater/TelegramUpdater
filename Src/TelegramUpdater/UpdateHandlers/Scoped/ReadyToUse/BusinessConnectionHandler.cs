namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.BusinessConnection"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class BusinessConnectionHandler(int group = 0)
    : AnyHandler<BusinessConnection>(x => x.BusinessConnection, group)
{
    // Add any specific properties or methods for BusinessConnection if needed.
}
