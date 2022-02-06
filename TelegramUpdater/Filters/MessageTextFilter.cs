using System;
using Telegram.Bot.Types;

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
            : base((x) => x.Text != null && filter(x.Text))
        { }
    }
}
