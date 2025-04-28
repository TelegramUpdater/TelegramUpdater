namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ChatBoost"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
/// <param name="group">Handling priority group, The lower the sooner to process.</param>
public abstract class ChatBoostHandler( )
    : AnyHandler<ChatBoostUpdated>(x => x.ChatBoost )
{
    // Add any specific properties or methods for ChatBoostUpdated if needed.
}
