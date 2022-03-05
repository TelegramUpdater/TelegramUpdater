namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract <see cref="IScopedUpdateHandler"/> for
/// <see cref="Update.Message"/>.
/// </summary>
public abstract class MessageHandler : AnyHandler<Message>
{
    /// <summary>
    /// You can set handling priority in here.
    /// </summary>
    /// <param name="group">Handling priority.</param>
    protected MessageHandler(int group = 0)
        : base(x => x.Message, group)
    {
    }
}
