namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <inheritdoc />
public sealed class BusinessConnectionChannel(
    TimeSpan timeOut, IFilter<UpdaterFilterInputs<BusinessConnection>>? filter = null)
    : DefaultChannel<BusinessConnection>(UpdateType.BusinessConnection, timeOut, u => u.BusinessConnection, filter)
{
}
