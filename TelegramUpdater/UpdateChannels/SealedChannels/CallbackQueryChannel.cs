using Telegram.Bot.Types;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    public class CallbackQueryChannel : CallbackQueryChannelAbs
    {
        private readonly Filter<CallbackQuery>? _filter;

        public CallbackQueryChannel(Filter<CallbackQuery>? filter = default)
            : base()
        {
            _filter = filter;
        }

        protected override bool ShouldChannel(CallbackQuery t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }
    }
}
