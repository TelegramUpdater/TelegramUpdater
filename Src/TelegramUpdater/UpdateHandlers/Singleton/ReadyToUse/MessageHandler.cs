using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.Message"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="MessageHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
public sealed class MessageHandler(
    Func<IContainer<Message>, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter = default)
    : AnyHandler<Message>(UpdateType.Message,
           callback,
           filter,
           x => x.Message)
{
}
