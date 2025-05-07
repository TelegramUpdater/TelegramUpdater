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
}
