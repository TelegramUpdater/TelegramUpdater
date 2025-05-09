namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.RemovedChatBoost"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class RemovedChatBoostHandler()
    : DefaultHandler<ChatBoostRemoved>(x => x.RemovedChatBoost)
{
    // Add any specific properties or methods for ChatBoostRemoved if needed.
}
