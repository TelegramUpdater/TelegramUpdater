namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.MyChatMember"/>.
/// </summary>
public sealed class MyChatMemberChannel : AnyChannel<ChatMemberUpdated>
{
    /// <summary>
    /// Initialize a new instance of <see cref="MyChatMemberChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public MyChatMemberChannel(TimeSpan timeOut, IFilter<ChatMemberUpdated>? filter)
        : base(UpdateType.MyChatMember, x => x.MyChatMember, timeOut, filter)
    {
    }
}
