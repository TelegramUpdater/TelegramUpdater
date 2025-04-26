namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class EditedBusinessMessageHandler(int group = 0)
    : AnyHandler<Message>(x => x.EditedBusinessMessage, group)
{
    // Add any specific properties or methods for EditedBusinessMessage if needed.
}
