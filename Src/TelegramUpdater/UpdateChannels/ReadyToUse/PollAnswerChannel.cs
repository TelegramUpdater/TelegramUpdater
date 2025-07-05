namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="PollAnswerChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public class PollAnswerChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<PollAnswer>>? filter)
    : DefaultChannel<PollAnswer>(UpdateType.PollAnswer, timeOut, x => x.PollAnswer, filter)
{
}
