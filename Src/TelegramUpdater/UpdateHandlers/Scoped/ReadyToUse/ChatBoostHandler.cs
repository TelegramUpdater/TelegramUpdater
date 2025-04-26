namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class ChatBoostHandler(int group = 0)
    : AnyHandler<ChatBoostUpdated>(x => x.ChatBoost, group)
{
    // Add any specific properties or methods for ChatBoostUpdated if needed.
}
