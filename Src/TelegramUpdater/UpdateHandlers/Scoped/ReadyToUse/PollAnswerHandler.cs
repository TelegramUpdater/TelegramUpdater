namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class PollAnswerHandler()
    : DefaultHandler<PollAnswer>(x => x.PollAnswer)
{
}
