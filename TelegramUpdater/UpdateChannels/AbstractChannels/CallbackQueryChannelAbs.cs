using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public abstract class CallbackQueryChannelAbs : AbstractChannel<CallbackQuery>
    {
        public override CallbackQuery? GetT(Update? update) => update?.CallbackQuery;
    }
}
