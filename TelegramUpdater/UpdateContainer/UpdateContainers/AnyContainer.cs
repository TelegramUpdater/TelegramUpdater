using System;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    public class AnyContainer<T> : UpdateContainerAbs<T> where T : class
    {
        public AnyContainer(
            Func<Update, T?> insiderResovler,
            IUpdater updater, Update insider) : base(insiderResovler, updater, insider)
        {
        }
    }
}
