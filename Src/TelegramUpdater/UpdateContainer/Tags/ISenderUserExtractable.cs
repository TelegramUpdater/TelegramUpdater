namespace TelegramUpdater.UpdateContainer.Tags;

/// <summary>
/// A sender of type <see cref="User"/> can be extracted from this object.
/// </summary>
public interface ISenderUserExtractable: ISenderIdExtractable
{
    /// <summary>
    /// Get the sender <see cref="User"/>.
    /// </summary>
    /// <returns></returns>
    public User? GetSenderUser();

    /// <summary>
    /// Ensured the sender user exists by throwing an exception.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public User GetEnsuredSenderUser()
    {
        return GetSenderUser()
            ?? throw new InvalidOperationException("User id is null!");
    }

    /// <summary>
    /// Ensured the sender user exists by throwing an exception.
    /// </summary>
    /// <remarks>
    /// This throws <see cref="ContinuePropagationException"/> which only stops current handler's processing.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public User GetSenderUserOrContinue()
    {
        return GetSenderUser()
            ?? throw new ContinuePropagationException();
    }
}
