namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.Poll"/>.
/// </summary>
public abstract class PollHandler : AnyHandler<Poll>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected PollHandler(int group) : base(x => x.Poll, group)
    {
    }
}
