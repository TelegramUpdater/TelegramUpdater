using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
public sealed class BusinessMessageHandler(
    Func<IContainer<Message>, Task> callback,
    Filter<UpdaterFilterInputs<Message>>? filter = default)
    : DefaultHandler<Message>(UpdateType.BusinessMessage, callback, filter, x => x.BusinessMessage)
{
}
