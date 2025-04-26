using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessConnection"/>.
/// </summary>
public sealed class BusinessConnectionHandler(
    Func<IContainer<BusinessConnection>, Task> callback,
    Filter<BusinessConnection>? filter = default,
    int group = 0) : AnyHandler<BusinessConnection>(UpdateType.BusinessConnection, x => x.BusinessConnection, callback, filter, group)
{
}
