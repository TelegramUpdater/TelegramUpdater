using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
public sealed class ChatBoostHandler : AnyHandler<ChatBoostUpdated>
{
    public ChatBoostHandler(
        Func<IContainer<ChatBoostUpdated>, Task> callback,
        Filter<ChatBoostUpdated>? filter = default,
        int group = 0)
        : base(UpdateType.ChatBoost, x => x.ChatBoost, callback, filter, group)
    {
    }
}
