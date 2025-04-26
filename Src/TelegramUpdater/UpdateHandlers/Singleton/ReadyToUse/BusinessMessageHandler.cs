using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
public sealed class BusinessMessageHandler(
    Func<IContainer<Message>, Task> callback,
    Filter<Message>? filter = default,
    int group = 0) : AnyHandler<Message>(UpdateType.BusinessMessage, x => x.BusinessMessage, callback, filter, group)
{
}
