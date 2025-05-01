using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
/// </summary>
public sealed class PurchasedPaidMediaHandler(
    Func<IContainer<PaidMediaPurchased>, Task> callback,
    Filter<UpdaterFilterInputs<PaidMediaPurchased>>? filter = default)
    : AnyHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia, callback, filter, x => x.PurchasedPaidMedia)
{
}
