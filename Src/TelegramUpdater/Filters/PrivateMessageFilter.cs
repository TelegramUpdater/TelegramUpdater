namespace TelegramUpdater.Filters;

/// <summary>
/// A filter that checks if the message sent to private chat.
/// </summary>
public class PrivateMessageFilter : UpdaterFilter<Message>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateMessageFilter"/> class.
    /// </summary>
    public PrivateMessageFilter()
        : base((c) => c.Chat.Type == ChatType.Private)
    {
    }
}
