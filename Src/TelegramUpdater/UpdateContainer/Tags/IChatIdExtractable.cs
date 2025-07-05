namespace TelegramUpdater.UpdateContainer.Tags;

/// <summary>
/// A <see cref="Chat.Id"/> of type <see cref="long"/> can be extracted from this object.
/// </summary>
public interface IChatIdExtractable
{
    /// <summary>
    /// Get the <see cref="Chat.Id"/> (<see cref="long"/>).
    /// </summary>
    /// <returns></returns>
    public long? GetChatId();

    /// <summary>
    /// Ensured the chat id exists by throwing an exception.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long GetEnsuredChatId()
    {
        return GetChatId()
            ?? throw new InvalidOperationException("User id is null!");
    }

    /// <summary>
    /// Ensured the chat id exists by throwing an exception.
    /// </summary>
    /// <remarks>
    /// This throws <see cref="ContinuePropagationException"/> which only stops current handler's processing.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long GetChatIdOrContinue()
    {
        return GetChatId()
            ?? throw new ContinuePropagationException();
    }
}
