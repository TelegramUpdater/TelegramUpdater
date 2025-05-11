using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class EditedBusinessMessageControllerHandler()
    : AbstractMessageControllerHandler(x => x.EditedBusinessMessage)
{
    // Add any specific properties or methods for EditedBusinessMessage if needed.
}
