namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public sealed class MessageReactionCountChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<MessageReactionCountUpdated>>? filter = null)
    : DefaultChannel<MessageReactionCountUpdated>(UpdateType.MessageReactionCount, timeOut, u => u.MessageReactionCount, filter)
{
}
