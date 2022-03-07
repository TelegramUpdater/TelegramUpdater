using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.Message"/>.
/// </summary>
public sealed class MessageHandler : AnyHandler<Message>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="MessageHandler"/>.
    /// </summary>
    /// <param name="callback">
    /// A callback function that will be called when an <see cref="Update"/>
    /// passes the <paramref name="filter"/>.
    /// </param>
    /// <param name="filter">
    /// A filter to choose the right update to be handled inside
    /// <paramref name="callback"/>.
    /// </param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public MessageHandler(
        Func<IContainer<Message>, Task> callback,
        IFilter<Message>? filter = default,
        int group = default)
        : base(UpdateType.Message,
               x => x.Message,
               callback,
               filter,
               group)
    { }
}
