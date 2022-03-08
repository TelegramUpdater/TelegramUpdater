namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChannelPost"/>.
/// </summary>
public abstract class ChannelPostHandler : AnyHandler<Message>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected ChannelPostHandler(int group) : base(x => x.ChannelPost, group)
    {
    }
}
