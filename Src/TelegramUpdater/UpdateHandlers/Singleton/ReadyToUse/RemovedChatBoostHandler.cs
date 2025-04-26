using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.RemovedChatBoost"/>.
/// </summary>
public sealed class RemovedChatBoostHandler : AnyHandler<ChatBoostRemoved>
{
    public RemovedChatBoostHandler(
        Func<IContainer<ChatBoostRemoved>, Task> callback,
        Filter<ChatBoostRemoved>? filter = default,
        int group = 0)
        : base(UpdateType.RemovedChatBoost, x => x.RemovedChatBoost, callback, filter, group)
    {
    }
}
