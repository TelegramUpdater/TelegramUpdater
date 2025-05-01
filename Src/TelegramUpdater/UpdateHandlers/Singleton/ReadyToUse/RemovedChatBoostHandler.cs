using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.RemovedChatBoost"/>.
/// </summary>
public sealed class RemovedChatBoostHandler(
    Func<IContainer<ChatBoostRemoved>, Task> callback,
    Filter<UpdaterFilterInputs<ChatBoostRemoved>>? filter = default)
    : AnyHandler<ChatBoostRemoved>(UpdateType.RemovedChatBoost, callback, filter, x => x.RemovedChatBoost)
{
}
