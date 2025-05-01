using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="ShippingQueryChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class ShippingQueryChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<ShippingQuery>>? filter)
    : DefaultChannel<ShippingQuery>(UpdateType.ShippingQuery, x=> x.ShippingQuery, timeOut, filter)
{
}
