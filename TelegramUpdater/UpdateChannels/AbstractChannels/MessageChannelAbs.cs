using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public abstract class MessageChannelAbs : AbstractChannel<Message>
    {
        protected MessageChannelAbs() : base()
        { }

        public override Message? GetT(Update? update) => update?.Message;
    }
}
