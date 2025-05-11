// Ignore Spelling: Inline

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ChosenInlineResultControllerHandler()
    : DefaultControllerHandler<ChosenInlineResult>(x => x.ChosenInlineResult)
{
}
