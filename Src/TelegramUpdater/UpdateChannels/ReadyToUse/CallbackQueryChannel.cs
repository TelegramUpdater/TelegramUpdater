namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public class CallbackQueryChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<CallbackQuery>>? filter = default)
    : DefaultChannel<CallbackQuery>(UpdateType.CallbackQuery, timeOut, x => x.CallbackQuery, filter)
{
}
