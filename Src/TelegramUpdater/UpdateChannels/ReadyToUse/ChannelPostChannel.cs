namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChannelPost"/>.
/// </summary>
public sealed class ChannelPostChannel : AnyChannel<Message>
{
    /// <summary>
    /// Initialize a new instance of <see cref="ChannelPostChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public ChannelPostChannel(TimeSpan timeOut, IFilter<Message>? filter)
        : base(UpdateType.ChannelPost, x => x.ChannelPost, timeOut, filter)
    {
    }
}
