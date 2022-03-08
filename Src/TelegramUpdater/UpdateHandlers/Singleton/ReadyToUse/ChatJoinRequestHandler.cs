using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChatJoinRequest"/>.
/// </summary>
public sealed class ChatJoinRequestHandler : AnyHandler<ChatJoinRequest>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="ChatJoinRequestHandler"/>.
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
    public ChatJoinRequestHandler(Func<IContainer<ChatJoinRequest>, Task> callback,
                                  IFilter<ChatJoinRequest>? filter,
                                  int group)
        : base(UpdateType.ChatJoinRequest, x=> x.ChatJoinRequest, callback, filter, group)
    {
    }
}
