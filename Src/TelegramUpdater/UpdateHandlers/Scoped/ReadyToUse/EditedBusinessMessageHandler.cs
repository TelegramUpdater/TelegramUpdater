namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedBusinessMessage"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class EditedBusinessMessageHandler()
    : AnyHandler<Message>(x => x.EditedBusinessMessage)
{
    // Add any specific properties or methods for EditedBusinessMessage if needed.
}
