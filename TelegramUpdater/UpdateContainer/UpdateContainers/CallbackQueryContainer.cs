using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    public class CallbackQueryContainer : UpdateContainerAbs<CallbackQuery>
    {
        public CallbackQueryContainer(IUpdater updater, Update insider)
            : base(x => x.CallbackQuery, updater, insider)
        {
        }
    }
}
