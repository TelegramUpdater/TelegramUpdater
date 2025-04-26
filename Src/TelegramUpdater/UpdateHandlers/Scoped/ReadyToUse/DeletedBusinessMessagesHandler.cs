namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class DeletedBusinessMessagesHandler(int group = 0)
    : AnyHandler<BusinessMessagesDeleted>(x => x.DeletedBusinessMessages, group)
{
    // Add any specific properties or methods for BusinessMessagesDeleted if needed.
}
