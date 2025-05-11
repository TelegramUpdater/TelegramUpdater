using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public abstract class EditedMessageControllerHandler()
    : AbstractMessageControllerHandler(x => x.EditedMessage)
{
}
