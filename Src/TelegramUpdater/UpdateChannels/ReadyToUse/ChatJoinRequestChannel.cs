namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChatJoinRequest"/>.
/// </summary>
public sealed class ChatJoinRequestChannel : AnyChannel<ChatJoinRequest>
{
    /// <summary>
    /// Initialize a new instance of <see cref="ChatJoinRequestChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public ChatJoinRequestChannel(TimeSpan timeOut, IFilter<ChatJoinRequest>? filter)
        : base(UpdateType.ChatJoinRequest, x => x.ChatJoinRequest, timeOut, filter)
    {
    }
}
