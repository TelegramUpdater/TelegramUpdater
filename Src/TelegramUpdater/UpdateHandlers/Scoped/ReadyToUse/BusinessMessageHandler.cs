using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.BusinessMessage"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class BusinessMessageHandler()
    : AbstractMessageHandler(x => x.BusinessMessage)
{
    // Add any specific properties or methods for BusinessMessage if needed.
}
