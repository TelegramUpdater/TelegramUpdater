using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class BusinessMessageControllerHandler()
    : AbstractMessageControllerHandler(x => x.BusinessMessage)
{
    // Add any specific properties or methods for BusinessMessage if needed.
}
