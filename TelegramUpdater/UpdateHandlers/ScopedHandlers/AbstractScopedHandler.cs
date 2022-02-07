using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    public abstract class AbstractScopedHandler<T> : IScopedUpdateHandler where T : class
    {
        private readonly Func<Update, T?> _getT;

        protected AbstractScopedHandler(Func<Update, T?> getT, int group)
        {
            Group = group;
            _getT = getT;
        }

        public int Group { get; }

        protected abstract Task HandleAsync(UpdateContainerAbs<T> updateContainer);

        protected abstract UpdateContainerAbs<T> ContainerBuilder(
            Updater updater, ITelegramBotClient botClient, Update update);

        protected T? GetT(Update update) => _getT(update);

        public async Task HandleAsync(Updater updater, ITelegramBotClient botClient, Update update)
            => await HandleAsync(
                ContainerBuilder(updater, botClient, update));
    }
}
