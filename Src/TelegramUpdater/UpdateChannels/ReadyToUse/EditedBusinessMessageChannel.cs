namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public class EditedBusinessMessageChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<Message>>? filter = null)
    : DefaultChannel<Message>(UpdateType.EditedBusinessMessage, timeOut, u => u.EditedBusinessMessage, filter)
{
}
