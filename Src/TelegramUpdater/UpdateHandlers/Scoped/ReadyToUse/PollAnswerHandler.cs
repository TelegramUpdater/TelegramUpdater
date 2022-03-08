namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
public abstract class PollAnswerHandler : AnyHandler<PollAnswer>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected PollAnswerHandler(int group = default)
        : base(x => x.PollAnswer, group)
    {
    }
}
