namespace TelegramUpdater.Filters
{
    public class MessageRepliedFilter : Filter<Message>
    {
        public MessageRepliedFilter()
            : base(x => x.ReplyToMessage != null)
        {
        }
    }
}
