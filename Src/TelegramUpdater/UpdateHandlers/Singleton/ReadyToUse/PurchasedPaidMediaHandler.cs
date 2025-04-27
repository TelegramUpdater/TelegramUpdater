using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
/// </summary>
public sealed class PurchasedPaidMediaHandler(
    Func<IContainer<PaidMediaPurchased>, Task> callback,
    Filter<UpdaterFilterInputs<PaidMediaPurchased>>? filter = default,
    int group = 0) : AnyHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia, x => x.PurchasedPaidMedia, callback, filter, group)
{
}
