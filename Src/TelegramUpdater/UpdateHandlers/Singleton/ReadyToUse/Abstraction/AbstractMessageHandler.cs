using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.Abstraction;

/// <summary>
/// Abstract scoped update handler for all handlers with <see cref="Message"/> input.
/// </summary>
/// <param name="updateType"></param>
/// <param name="callback"></param>
/// <param name="getT"></param>
/// <param name="endpoint"></param>
/// <param name="filter"></param>
public abstract class AbstractMessageHandler(
    UpdateType updateType,
    Func<MessageContainer, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter = null,
    Func<Update, Message?>? getT = null,
    bool endpoint = true)
    : AbstractSingletonUpdateHandler<Message, MessageContainer>(updateType, getT, filter, endpoint)
{
    internal override MessageContainer ContainerBuilder(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        => new(updater, shiningInfo, ExtraData);

    /// <inheritdoc/>
    protected override Task HandleAsync(MessageContainer updateContainer) => callback(updateContainer);
}
