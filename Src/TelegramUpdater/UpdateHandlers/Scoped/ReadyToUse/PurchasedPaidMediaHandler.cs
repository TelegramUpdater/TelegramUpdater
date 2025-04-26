namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class PurchasedPaidMediaHandler(int group = 0)
    : AnyHandler<PaidMediaPurchased>(x => x.PurchasedPaidMedia, group)
{
    // Add any specific properties or methods for PaidMediaPurchased if needed.
}
