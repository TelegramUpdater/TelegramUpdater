// Ignore Spelling: Inline

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ChosenInlineResultHandler()
    : DefaultHandler<ChosenInlineResult>(x => x.ChosenInlineResult)
{
}
