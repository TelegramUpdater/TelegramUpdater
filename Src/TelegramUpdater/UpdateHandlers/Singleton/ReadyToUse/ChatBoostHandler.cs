using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
public sealed class ChatBoostHandler(
    Func<IContainer<ChatBoostUpdated>, Task> callback,
    Filter<UpdaterFilterInputs<ChatBoostUpdated>>? filter = default,
    int group = 0) : AnyHandler<ChatBoostUpdated>(UpdateType.ChatBoost, x => x.ChatBoost, callback, filter, group)
{
}
