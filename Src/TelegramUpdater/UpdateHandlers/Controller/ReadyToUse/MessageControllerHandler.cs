// Ignore Spelling: Webpage

using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.Message"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MessageControllerHandler()
    : AbstractMessageControllerHandler(x => x.Message)
{
}
