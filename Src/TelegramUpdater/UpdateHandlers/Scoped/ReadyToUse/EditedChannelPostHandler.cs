namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
public abstract class EditedChannelPostHandler()
    : DefaultHandler<Message>(x => x.EditedChannelPost)
{
}
