namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChatMember"/>.
/// </summary>
public sealed class ChatMemberChannel : AnyChannel<ChatMemberUpdated>
{
    /// <summary>
    /// Initialize a new instance of <see cref="ChatMemberChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public ChatMemberChannel(TimeSpan timeOut, IFilter<ChatMemberUpdated>? filter)
        : base(UpdateType.ChatMember, x => x.ChatMember, timeOut, filter)
    {
    }
}
