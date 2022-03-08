namespace TelegramUpdater.Filters
{
    /// <summary>
    /// A filter that checks if the message is a reply to another message.
    /// </summary>
    public class MessageRepliedFilter : Filter<Message>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepliedFilter"/> class.
        /// </summary>
        public MessageRepliedFilter()
            : base(x => x.ReplyToMessage != null)
        {
        }
    }
}
