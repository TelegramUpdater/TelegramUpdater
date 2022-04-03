namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.Poll"/>.
/// </summary>
public sealed class PollChannel : AnyChannel<Poll>
{
    /// <summary>
    /// Initialize a new instance of <see cref="PollChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public PollChannel(TimeSpan timeOut, IFilter<Poll>? filter)
        : base(UpdateType.Poll, x => x.Poll, timeOut, filter)
    {
    }
}
