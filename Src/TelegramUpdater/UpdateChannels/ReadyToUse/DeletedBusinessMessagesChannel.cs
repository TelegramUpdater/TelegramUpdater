namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public class DeletedBusinessMessagesChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<BusinessMessagesDeleted>>? filter = null)
    : DefaultChannel<BusinessMessagesDeleted>(UpdateType.DeletedBusinessMessages, timeOut, u => u.DeletedBusinessMessages, filter)
{
}
