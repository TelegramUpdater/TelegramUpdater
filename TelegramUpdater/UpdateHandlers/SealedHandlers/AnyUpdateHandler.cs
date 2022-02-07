using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public class AnyUpdateHandler<T> : AbstractHandler<T> where T : class
    {
        private readonly Func<UpdateContainerAbs<T>, Task> _handleAsync;

        public AnyUpdateHandler(
            UpdateType updateType,
            Func<Update, T?> getT,
            Func<UpdateContainerAbs<T>, Task> callbak,
            Filter<T>? filter,
            int group) : base(updateType, getT, filter, group)
        {
            _handleAsync = callbak;
        }

        protected override UpdateContainerAbs<T> ContainerBuilder(
            Updater updater, ITelegramBotClient botClient, Update update)
                => new AnyContainer<T>(GetT, updater, update, botClient);

        protected override async Task HandleAsync(UpdateContainerAbs<T> updateContainer)
        {
            await _handleAsync(updateContainer);
        }
    }
}
