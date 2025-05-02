using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChatJoinRequest"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="ChatJoinRequestHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
/// <param name="endpoint"></param>
public sealed class ChatJoinRequestHandler(
    Func<IContainer<ChatJoinRequest>, Task> callback,
    IFilter<UpdaterFilterInputs<ChatJoinRequest>>? filter,
    bool endpoint = true)
    : DefaultHandler<ChatJoinRequest>(
        UpdateType.ChatJoinRequest,
        callback,
        filter,
        x => x.ChatJoinRequest,
        endpoint)
{
}
