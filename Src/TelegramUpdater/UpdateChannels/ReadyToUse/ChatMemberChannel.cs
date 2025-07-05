namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChatMember"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="ChatMemberChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public class ChatMemberChannel(TimeSpan timeOut, IFilter<UpdaterFilterInputs<ChatMemberUpdated>>? filter)
    : DefaultChannel<ChatMemberUpdated>(UpdateType.ChatMember, timeOut, x => x.ChatMember, filter)
{
}
