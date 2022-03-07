﻿namespace TelegramUpdater.Filters
{
    /// <summary>
    /// A filter that checks if the message sent to private chat.
    /// </summary>
    public class PrivateMessageFilter : Filter<Message>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateMessageFilter"/> class.
        /// </summary>
        public PrivateMessageFilter()
            : base(c => c.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
        {
        }
    }
}
