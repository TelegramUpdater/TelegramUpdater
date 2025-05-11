namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class PollAnswerControllerHandler()
    : DefaultControllerHandler<PollAnswer>(x => x.PollAnswer)
{
}
