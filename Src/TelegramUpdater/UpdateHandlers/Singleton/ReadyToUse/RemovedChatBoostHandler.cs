using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.RemovedChatBoost"/>.
/// </summary>
public sealed class RemovedChatBoostHandler(
    Func<IContainer<ChatBoostRemoved>, Task> callback,
    IFilter<UpdaterFilterInputs<ChatBoostRemoved>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<ChatBoostRemoved>(UpdateType.RemovedChatBoost, callback, filter, x => x.RemovedChatBoost, endpoint)
{
}
