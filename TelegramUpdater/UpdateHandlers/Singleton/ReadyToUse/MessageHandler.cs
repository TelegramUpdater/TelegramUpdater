using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// An update handler for <see cref="Update.Message"/>.
/// </summary>
public sealed class MessageHandler : AnyHandler<Message>
{
    /// <summary>
    /// Create update handler for <see cref="Message"/>.
    /// </summary>
    /// <param name="filter">Filters for this handler.</param>
    /// <param name="group">Handling priority for this handler.</param>
    /// <param name="callback">
    /// A callback function where you may handle the incoming update.
    /// </param>
    public MessageHandler(
        Func<IContainer<Message>, Task> callback,
        IFilter<Message>? filter = default,
        int group = 0)
        : base(UpdateType.Message,
               x => x.Message,
               callback,
               filter,
               group)
    { }
}
