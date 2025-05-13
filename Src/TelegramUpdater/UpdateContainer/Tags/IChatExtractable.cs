namespace TelegramUpdater.UpdateContainer.Tags;

/// <summary>
/// A chat of type <see cref="Chat"/> can be extracted from this object.
/// </summary>
public interface IChatExtractable: IChatIdExtractable
{
    /// <summary>
    /// Get the <see cref="Chat"/>.
    /// </summary>
    /// <returns></returns>
    public Chat? GetChat();

    /// <summary>
    /// Ensured the chat exists by throwing an exception.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Chat? GetEnsuredChat()
    {
        return GetChat()
            ?? throw new InvalidOperationException("User id is null!");
    }

    /// <summary>
    /// Ensured the chat exists by throwing an exception.
    /// </summary>
    /// <remarks>
    /// This throws <see cref="ContinuePropagationException"/> which only stops current handler's proccesing.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Chat? GetChatOrContinue()
    {
        return GetChat()
            ?? throw new ContinuePropagationException();
    }
}
