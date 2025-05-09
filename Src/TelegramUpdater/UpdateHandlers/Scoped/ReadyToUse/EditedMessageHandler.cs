using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public abstract class EditedMessageHandler()
    : AbstractMessageHandler(x => x.EditedMessage)
{
}
