using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
public sealed class EditedBusinessMessageHandler : AnyHandler<Message>
{
    public EditedBusinessMessageHandler(
        Func<IContainer<Message>, Task> callback,
        Filter<Message>? filter = default,
        int group = 0)
        : base(UpdateType.EditedBusinessMessage, x => x.EditedBusinessMessage, callback, filter, group)
    {
    }
}
