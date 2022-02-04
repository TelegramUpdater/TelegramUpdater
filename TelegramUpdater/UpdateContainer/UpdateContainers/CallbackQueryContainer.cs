using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    public class CallbackQueryContainer : UpdateContainerAbs<CallbackQuery>
    {
        public CallbackQueryContainer(Updater updater, Update insider, ITelegramBotClient botClient)
            : base(x=> x.CallbackQuery, updater, insider, botClient)
        {
        }
    }
}
