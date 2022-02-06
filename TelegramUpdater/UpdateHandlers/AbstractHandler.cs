using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers
{
    public abstract class AbstractHandler<T> : ISingletonUpdateHandler where T : class
    {
        protected AbstractHandler(int group)
        {
            Group = group;
        }

        public abstract UpdateType UpdateType { get; }

        public int Group { get; }

        // TODO: implement filter here.

        protected abstract bool ShouldHandle(T t);

        protected abstract Task HandleAsync(UpdateContainerAbs<T> updateContainer);

        protected abstract T? GetT(Update update);

        protected abstract UpdateContainerAbs<T> ContainerBuilder(
            Updater updater, ITelegramBotClient botClient, Update update);

        public async Task HandleAsync(Updater updater, ITelegramBotClient botClient, Update update)
            => await HandleAsync(
                ContainerBuilder(updater, botClient, update));

        public bool ShouldHandle(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }
    }
}
