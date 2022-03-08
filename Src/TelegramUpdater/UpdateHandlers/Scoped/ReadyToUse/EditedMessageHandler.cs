namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public abstract class EditedMessageHandler : AnyHandler<Message>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    public EditedMessageHandler(int group = default)
        : base(x => x.EditedMessage, group)
    {
    }
}
