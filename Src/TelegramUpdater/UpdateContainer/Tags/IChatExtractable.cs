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
}
