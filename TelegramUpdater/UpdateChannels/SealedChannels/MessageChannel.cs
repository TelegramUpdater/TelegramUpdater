using Telegram.Bot.Types;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    public sealed class MessageChannel : MessageChannelAbs
    {
        public MessageChannel(Filter<Message>? filter) : base(filter)
        {
        }
    }
}
