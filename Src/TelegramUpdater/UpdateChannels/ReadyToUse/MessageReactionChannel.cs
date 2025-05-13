namespace TelegramUpdater.UpdateChannels.ReadyToUse;


/// <inheritdoc />
public sealed class MessageReactionChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<MessageReactionUpdated>>? filter = null)
    : DefaultChannel<MessageReactionUpdated>(UpdateType.MessageReaction, timeOut, u => u.MessageReaction, filter)
{
}
