namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public abstract class EditedMessageHandler()
    : DefaultHandler<Message>(x => x.EditedMessage)
{
}
