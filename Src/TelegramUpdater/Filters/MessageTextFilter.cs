namespace TelegramUpdater.Filters
{
    /// <summary>
    /// A filter on <see cref="Message.Text"/>
    /// </summary>
    public class MessageTextFilter : Filter<Message>
    {
        /// <summary>
        /// A filter on <see cref="Message.Text"/>
        /// </summary>
        public MessageTextFilter(Func<string, bool> filter)
            : base((_, x) => x.Text != null && filter(x.Text))
        { }
    }
}
