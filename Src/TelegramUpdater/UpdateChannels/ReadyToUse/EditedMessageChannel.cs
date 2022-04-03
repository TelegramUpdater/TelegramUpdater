namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public sealed class EditedMessageChannel : AnyChannel<Message>
{
    /// <summary>
    /// Initialize a new instance of <see cref="EditedMessageChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public EditedMessageChannel(TimeSpan timeOut, IFilter<Message>? filter)
        : base(UpdateType.EditedMessage, x => x.EditedMessage, timeOut, filter)
    {
    }
}
