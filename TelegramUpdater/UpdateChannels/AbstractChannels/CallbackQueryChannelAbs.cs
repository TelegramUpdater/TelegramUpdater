using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public abstract class CallbackQueryChannelAbs : AbstractChannel<CallbackQuery>
    {
        protected CallbackQueryChannelAbs(Filter<CallbackQuery>? filter)
            : base(UpdateType.CallbackQuery, x => x.CallbackQuery, filter)
        { }
    }
}
