using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
/// </summary>
public sealed class PurchasedPaidMediaHandler : AnyHandler<PaidMediaPurchased>
{
    public PurchasedPaidMediaHandler(
        Func<IContainer<PaidMediaPurchased>, Task> callback,
        Filter<PaidMediaPurchased>? filter = default,
        int group = 0)
        : base(UpdateType.PurchasedPaidMedia, x => x.PurchasedPaidMedia, callback, filter, group)
    {
    }
}
