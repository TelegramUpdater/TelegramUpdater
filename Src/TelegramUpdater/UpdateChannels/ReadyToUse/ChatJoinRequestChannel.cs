namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.ChatJoinRequest"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="ChatJoinRequestChannel"/>
/// to use as <see cref="IGenericUpdateChannel{T}"/>.
/// </remarks>
/// <param name="timeOut">Timeout to wait for channel.</param>
/// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
public sealed class ChatJoinRequestChannel(TimeSpan timeOut, IFilter<ChatJoinRequest>? filter)
    : AnyChannel<ChatJoinRequest>(UpdateType.ChatJoinRequest, x => x.ChatJoinRequest, timeOut, filter)
{
}
