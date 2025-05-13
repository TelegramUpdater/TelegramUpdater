namespace TelegramUpdater.UpdateContainer.Tags;

/// <summary>
/// A sender of type <see cref="Chat"/> can be extracted from this object.
/// </summary>
/// <remarks>
/// As instance for messages sent in behalf of a chat.
/// </remarks>
public interface ISenderChatExtractable: ISenderIdExtractable
{
    /// <summary>
    /// Get the sender <see cref="Chat"/>.
    /// </summary>
    /// <returns></returns>
    public Chat? GetSenderChat();

    /// <summary>
    /// Ensured the sender chat id exists by throwing an exception.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Chat GetEnsuredSenderChat()
    {
        return GetSenderChat()
            ?? throw new InvalidOperationException("User id is null!");
    }

    /// <summary>
    /// Ensured the sender chat id exists by throwing an exception.
    /// </summary>
    /// <remarks>
    /// This throws <see cref="ContinuePropagationException"/> which only stops current handler's processing.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Chat GetSenderChatOrContinue()
    {
        return GetSenderChat()
            ?? throw new ContinuePropagationException();
    }
}
