namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
public sealed class PollAnswerChannel : AnyChannel<PollAnswer>
{
    /// <summary>
    /// Initialize a new instance of <see cref="PollAnswerChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public PollAnswerChannel(TimeSpan timeOut, IFilter<PollAnswer>? filter)
        : base(UpdateType.PollAnswer, x => x.PollAnswer, timeOut, filter)
    {
    }
}
