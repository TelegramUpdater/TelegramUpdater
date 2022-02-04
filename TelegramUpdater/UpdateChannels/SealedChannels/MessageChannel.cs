using Telegram.Bot.Types;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    public sealed class MessageChannel : MessageChannelAbs
    {
        private readonly Filter<Message>? _filter;

        public MessageChannel(Filter<Message>? filter = default)
            : base()
        {
            _filter = filter;
        }

        protected override bool ShouldChannel(Message t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }
    }
}
