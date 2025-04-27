namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.Poll"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="PollChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class PollChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<Poll>>? filter)
    : AnyChannel<Poll>(UpdateType.Poll, x => x.Poll, timeOut, filter)
{
}
