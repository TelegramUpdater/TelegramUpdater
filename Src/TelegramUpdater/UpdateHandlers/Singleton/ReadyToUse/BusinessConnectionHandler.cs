using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessConnection"/>.
/// </summary>
public sealed class BusinessConnectionHandler(
    Func<IContainer<BusinessConnection>, Task> callback,
    IFilter<UpdaterFilterInputs<BusinessConnection>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<BusinessConnection>(
        UpdateType.BusinessConnection,
        callback,
        filter,
        x => x.BusinessConnection,
        endpoint)
{
}
