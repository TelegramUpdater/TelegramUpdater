namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.RemovedChatBoost"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class RemovedChatBoostHandler(int group = 0)
    : AnyHandler<ChatBoostRemoved>(x => x.RemovedChatBoost, group)
{
    // Add any specific properties or methods for ChatBoostRemoved if needed.
}
