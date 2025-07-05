namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public class ChatBoostChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<ChatBoostUpdated>>? filter = null)
    : DefaultChannel<ChatBoostUpdated>(UpdateType.ChatBoost, timeOut, u => u.ChatBoost, filter)
{
}
