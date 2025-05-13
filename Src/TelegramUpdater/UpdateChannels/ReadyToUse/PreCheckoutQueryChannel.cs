using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.PreCheckoutQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="PreCheckoutQueryChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class PreCheckoutQueryChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<PreCheckoutQuery>>? filter)
    : DefaultChannel<PreCheckoutQuery>(UpdateType.PreCheckoutQuery, timeOut, x => x.PreCheckoutQuery, filter)
{
}
