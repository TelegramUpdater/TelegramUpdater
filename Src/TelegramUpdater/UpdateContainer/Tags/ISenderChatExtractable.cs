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
}
