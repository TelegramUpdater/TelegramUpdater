using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
public sealed class EditedBusinessMessageHandler(
    Func<IContainer<Message>, Task> callback,
    Filter<UpdaterFilterInputs<Message>>? filter = default)
    : AnyHandler<Message>(UpdateType.EditedBusinessMessage, callback, filter, x => x.EditedBusinessMessage)
{
}
