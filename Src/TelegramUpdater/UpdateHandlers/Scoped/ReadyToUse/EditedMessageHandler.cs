namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public abstract class EditedMessageHandler(int group = default)
    : AnyHandler<Message>(x => x.EditedMessage, group)
{
}
