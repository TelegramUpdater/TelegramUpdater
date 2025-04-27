using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="EditedMessageHandler"/>.
/// </remarks>
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
public sealed class EditedMessageHandler(
    Func<IContainer<Message>, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter,
    int group) : AnyHandler<Message>(UpdateType.EditedMessage, x=> x.EditedMessage, callback, filter, group)
{
}
