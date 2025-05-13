namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public sealed class DeletedBusinessMessagesChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<BusinessMessagesDeleted>>? filter = null)
    : DefaultChannel<BusinessMessagesDeleted>(UpdateType.DeletedBusinessMessages, timeOut, u => u.DeletedBusinessMessages, filter)
{
}
