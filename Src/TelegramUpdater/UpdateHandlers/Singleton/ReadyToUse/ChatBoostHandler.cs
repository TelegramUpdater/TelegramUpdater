using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Singleton update handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
public class ChatBoostHandler(
    Func<IContainer<ChatBoostUpdated>, Task> callback,
    IFilter<UpdaterFilterInputs<ChatBoostUpdated>>? filter = default,
    bool endpoint = true)
    : DefaultHandler<ChatBoostUpdated>(
        UpdateType.ChatBoost,
        callback,
        filter,
        x => x.ChatBoost,
        endpoint)
{
}
