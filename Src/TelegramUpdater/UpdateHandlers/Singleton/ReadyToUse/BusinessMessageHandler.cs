using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
public sealed class BusinessMessageHandler(
    Func<IContainer<Message>, Task> callback,
    Filter<UpdaterFilterInputs<Message>>? filter = default) : AnyHandler<Message>(UpdateType.BusinessMessage, x => x.BusinessMessage, callback, filter )
{
}
