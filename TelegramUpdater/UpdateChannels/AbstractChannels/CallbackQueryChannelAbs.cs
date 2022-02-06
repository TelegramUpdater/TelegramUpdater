using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public abstract class CallbackQueryChannelAbs : AbstractChannel<CallbackQuery>
    {
        public override CallbackQuery? GetT(Update? update) => update?.CallbackQuery;

        public override UpdateType UpdateType => UpdateType.CallbackQuery;
    }
}
