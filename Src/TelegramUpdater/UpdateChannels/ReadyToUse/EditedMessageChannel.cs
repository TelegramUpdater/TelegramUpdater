namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="EditedMessageChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class EditedMessageChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<Message>>? filter)
    : DefaultChannel<Message>(UpdateType.EditedMessage, x => x.EditedMessage, timeOut, filter)
{
}
