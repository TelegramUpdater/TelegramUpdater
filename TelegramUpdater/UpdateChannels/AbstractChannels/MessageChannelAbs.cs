using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public abstract class MessageChannelAbs : AbstractChannel<Message>
    {
        protected MessageChannelAbs(Filter<Message>? filter)
            : base(UpdateType.Message, x => x.Message, filter)
        { }
    }
}
