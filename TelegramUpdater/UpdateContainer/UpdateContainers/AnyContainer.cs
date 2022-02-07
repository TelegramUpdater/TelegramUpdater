using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    public class AnyContainer<T> : UpdateContainerAbs<T> where T : class
    {
        public AnyContainer(
            Func<Update, T?> insiderResovler,
            Updater updater, Update insider,
            ITelegramBotClient botClient) : base(insiderResovler, updater, insider, botClient)
        {
        }
    }
}
