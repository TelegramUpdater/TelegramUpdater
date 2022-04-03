using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
public sealed class ShippingQueryChannel : AnyChannel<ShippingQuery>
{
    /// <summary>
    /// Initialize a new instance of <see cref="ShippingQueryChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public ShippingQueryChannel(TimeSpan timeOut, IFilter<ShippingQuery>? filter)
        : base(UpdateType.ShippingQuery, x=> x.ShippingQuery, timeOut, filter)
    {
    }
}
