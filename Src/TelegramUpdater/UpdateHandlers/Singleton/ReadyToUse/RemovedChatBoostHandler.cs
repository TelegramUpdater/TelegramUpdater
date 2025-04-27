using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.RemovedChatBoost"/>.
/// </summary>
public sealed class RemovedChatBoostHandler(
    Func<IContainer<ChatBoostRemoved>, Task> callback,
    Filter<UpdaterFilterInputs<ChatBoostRemoved>>? filter = default,
    int group = 0) : AnyHandler<ChatBoostRemoved>(UpdateType.RemovedChatBoost, x => x.RemovedChatBoost, callback, filter, group)
{
}
