// Ignore Spelling: Inline

namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.InlineQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="InlineQueryChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class InlineQueryChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<InlineQuery>>? filter)
    : DefaultChannel<InlineQuery>(UpdateType.InlineQuery,
        x => x.InlineQuery,
        timeOut,
        filter)
{
}
