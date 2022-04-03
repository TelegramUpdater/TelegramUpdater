using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.PreCheckoutQuery"/>.
/// </summary>
public sealed class PreCheckoutQueryChannel : AnyChannel<PreCheckoutQuery>
{
    /// <summary>
    /// Initialize a new instance of <see cref="PreCheckoutQueryChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public PreCheckoutQueryChannel(TimeSpan timeOut, IFilter<PreCheckoutQuery>? filter)
        : base(UpdateType.PreCheckoutQuery, x=> x.PreCheckoutQuery, timeOut, filter)
    {
    }
}
