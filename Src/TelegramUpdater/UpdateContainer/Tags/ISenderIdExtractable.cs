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
}
