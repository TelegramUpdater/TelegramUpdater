namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.CallbackQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="CallbackQueryChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class CallbackQueryChannel(
    TimeSpan timeOut,
    IFilter<CallbackQuery>? filter = default)
    : AnyChannel<CallbackQuery>(
        UpdateType.CallbackQuery,
        x => x.CallbackQuery,
        timeOut,
        filter)
{
}
