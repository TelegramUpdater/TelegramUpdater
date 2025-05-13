namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public sealed class BusinessMessageChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<Message>>? filter = null)
    : DefaultChannel<Message>(UpdateType.BusinessMessage, timeOut, u => u.BusinessMessage, filter)
{
}
