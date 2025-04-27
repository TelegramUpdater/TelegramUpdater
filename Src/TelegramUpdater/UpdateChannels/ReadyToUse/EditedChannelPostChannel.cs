namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="EditedChannelPostChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class EditedChannelPostChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<Message>>? filter)
    : AnyChannel<Message>(UpdateType.EditedChannelPost, x => x.EditedChannelPost, timeOut, filter)
{
}
