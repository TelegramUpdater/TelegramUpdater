namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public sealed class RemovedChatBoostChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<ChatBoostRemoved>>? filter = null)
    : DefaultChannel<ChatBoostRemoved>(UpdateType.RemovedChatBoost, timeOut, u => u.RemovedChatBoost, filter)
{
}
