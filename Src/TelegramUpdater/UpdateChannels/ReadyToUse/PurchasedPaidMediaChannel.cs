using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public sealed class PurchasedPaidMediaChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<PaidMediaPurchased>>? filter = null)
    : DefaultChannel<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia, timeOut, u => u.PurchasedPaidMedia, filter)
{
}
