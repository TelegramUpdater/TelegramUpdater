namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
public abstract class ChosenInlineResultHandler : AnyHandler<ChosenInlineResult>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected ChosenInlineResultHandler(int group = default)
        : base(x => x.ChosenInlineResult, group)
    {
    }
}
