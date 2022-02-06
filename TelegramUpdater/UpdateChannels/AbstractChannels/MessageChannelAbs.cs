using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public abstract class MessageChannelAbs : AbstractChannel<Message>
    {
        protected MessageChannelAbs() : base()
        { }

        public override Message? GetT(Update? update) => update?.Message;

        public override UpdateType UpdateType => UpdateType.Message;
    }
}
