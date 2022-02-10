using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    public sealed class MessageChannel : AnyChannelAbs<Message>
    {
        public MessageChannel(Filter<Message>? filter)
            : base(UpdateType.Message, x => x.Message, filter)
        {
        }
    }
}
