using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class PurchasedPaidMediaControllerHandler()
    : DefaultControllerHandler<PaidMediaPurchased>(x => x.PurchasedPaidMedia)
{
    // Add any specific properties or methods for PaidMediaPurchased if needed.
}
