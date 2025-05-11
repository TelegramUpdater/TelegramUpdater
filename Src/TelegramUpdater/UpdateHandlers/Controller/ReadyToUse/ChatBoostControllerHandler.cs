namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ChatBoostControllerHandler()
    : DefaultControllerHandler<ChatBoostUpdated>(x => x.ChatBoost)
{
    // Add any specific properties or methods for ChatBoostUpdated if needed.
}
