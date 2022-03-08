namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
public abstract class EditedChannelPostHandler : AnyHandler<Message>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    public EditedChannelPostHandler(int group)
        : base(x => x.EditedChannelPost, group)
    {
    }
}
