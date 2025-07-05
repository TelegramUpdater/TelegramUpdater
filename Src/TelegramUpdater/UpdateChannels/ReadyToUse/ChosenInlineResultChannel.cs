// Ignore Spelling: Inline

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="ChosenInlineResultChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public class ChosenInlineResultChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<ChosenInlineResult>>? filter)
    : DefaultChannel<ChosenInlineResult>(UpdateType.ChosenInlineResult, timeOut, x => x.ChosenInlineResult, filter)
{
}
