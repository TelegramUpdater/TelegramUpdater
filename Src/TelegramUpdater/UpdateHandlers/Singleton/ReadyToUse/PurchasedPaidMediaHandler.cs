using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
/// </summary>
public class PurchasedPaidMediaHandler(
    Func<IContainer<PaidMediaPurchased>, Task> callback,
    IFilter<UpdaterFilterInputs<PaidMediaPurchased>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia, callback, filter, x => x.PurchasedPaidMedia, endpoint)
{
}
