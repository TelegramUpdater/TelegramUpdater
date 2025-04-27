using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
public sealed class EditedBusinessMessageHandler(
    Func<IContainer<Message>, Task> callback,
    Filter<UpdaterFilterInputs<Message>>? filter = default,
    int group = 0) : AnyHandler<Message>(UpdateType.EditedBusinessMessage, x => x.EditedBusinessMessage, callback, filter, group)
{
}
