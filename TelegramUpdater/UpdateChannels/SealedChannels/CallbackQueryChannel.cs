using Telegram.Bot.Types;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    public sealed class CallbackQueryChannel : CallbackQueryChannelAbs
    {
        public CallbackQueryChannel(Filter<CallbackQuery>? filter = default)
            : base(filter)
        { }
    }
}
