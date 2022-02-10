using System;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    public abstract class AnyScopedHandler<T> : AbstractScopedHandler<T> where T : class
    {
        public AnyScopedHandler(Func<Update, T?> getT, int group) : base(getT, group)
        {
        }

        protected sealed override IContainer<T> ContainerBuilder(
            IUpdater updater, Update update)
        {
            return new AnyContainer<T>(GetT, updater, update);
        }
    }
}
