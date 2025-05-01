using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
public sealed class ChatBoostHandler(
    Func<IContainer<ChatBoostUpdated>, Task> callback,
    Filter<UpdaterFilterInputs<ChatBoostUpdated>>? filter = default)
    : AnyHandler<ChatBoostUpdated>(UpdateType.ChatBoost, callback, filter, x => x.ChatBoost)
{
}
