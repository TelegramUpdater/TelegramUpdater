using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    public abstract class AbstractScopedHandler<T> : IScopedUpdateHandler where T : class
    {
        protected AbstractScopedHandler(int group)
        {
            Group = group;
        }

        public int Group { get; }

        protected abstract Task HandleAsync(UpdateContainerAbs<T> updateContainer);

        protected abstract T? GetT(Update update);

        protected abstract UpdateContainerAbs<T> ContainerBuilder(
            Updater updater, ITelegramBotClient botClient, Update update);

        public async Task HandleAsync(Updater updater, ITelegramBotClient botClient, Update update)
            => await HandleAsync(
                ContainerBuilder(updater, botClient, update));
    }
}
