namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
public sealed class ChosenInlineResultChannel : AnyChannel<ChosenInlineResult>
{
    /// <summary>
    /// Initialize a new instance of <see cref="ChosenInlineResultChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public ChosenInlineResultChannel(TimeSpan timeOut, IFilter<ChosenInlineResult>? filter)
        : base(UpdateType.ChosenInlineResult, x => x.ChosenInlineResult, timeOut, filter)
    {
    }
}
