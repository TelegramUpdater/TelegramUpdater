namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChannelPost"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="ChannelPostChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class ChannelPostChannel(TimeSpan timeOut, IFilter<Message>? filter)
    : AnyChannel<Message>(UpdateType.ChannelPost, x => x.ChannelPost, timeOut, filter)
{
}
