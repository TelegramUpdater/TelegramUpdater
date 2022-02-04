using Telegram.Bot.Types;

namespace TelegramUpdater.Filters
{
    public class PrivateMessageFilter : Filter<Message>
    {
        public PrivateMessageFilter()
            : base(c => c.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
        {
        }
    }
}
