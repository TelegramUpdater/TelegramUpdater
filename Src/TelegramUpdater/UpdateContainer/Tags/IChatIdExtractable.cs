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
}
