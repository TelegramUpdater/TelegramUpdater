namespace TelegramUpdater.UpdateContainer.Tags;

/// <summary>
/// A sender id of type <see cref="long"/> can be extracted from this object.
/// </summary>
/// <remarks>
/// It's either a <see cref="User.Id"/> or <see cref="Chat.Id"/>.
/// </remarks>
public interface ISenderIdExtractable
{
    /// <summary>
    /// Get the sender id (<see cref="long"/>).
    /// </summary>
    /// <returns></returns>
    public long? GetSenderId();

    /// <summary>
    /// Ensured the sender id exists by throwing an exception.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long GetEnsuredSenderId()
    {
        return GetSenderId()
            ?? throw new InvalidOperationException("User id is null!");
    }

    /// <summary>
    /// Ensured the sender id exists by throwing an exception.
    /// </summary>
    /// <remarks>
    /// This throws <see cref="ContinuePropagationException"/> which only stops current handler's processing.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long GetSenderIdOrContinue()
    {
        return GetSenderId()
            ?? throw new ContinuePropagationException();
    }
}
