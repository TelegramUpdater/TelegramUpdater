namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
public sealed class EditedChannelPostChannel : AnyChannel<Message>
{
    /// <summary>
    /// Initialize a new instance of <see cref="EditedChannelPostChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public EditedChannelPostChannel(TimeSpan timeOut, IFilter<Message>? filter)
        : base(UpdateType.EditedChannelPost, x => x.EditedChannelPost, timeOut, filter)
    {
    }
}
