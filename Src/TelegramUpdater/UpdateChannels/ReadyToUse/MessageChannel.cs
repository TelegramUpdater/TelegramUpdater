namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.Message"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="MessageChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class MessageChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<Message>>? filter = default)
    : DefaultChannel<Message>(UpdateType.Message, x => x.Message, timeOut, filter)
{
}
