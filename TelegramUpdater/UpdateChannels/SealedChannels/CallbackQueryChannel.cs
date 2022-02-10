using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    public sealed class CallbackQueryChannel : AnyChannelAbs<CallbackQuery>
    {
        public CallbackQueryChannel(Filter<CallbackQuery>? filter)
            : base(UpdateType.CallbackQuery, x => x.CallbackQuery, filter)
        {
        }
    }
}
