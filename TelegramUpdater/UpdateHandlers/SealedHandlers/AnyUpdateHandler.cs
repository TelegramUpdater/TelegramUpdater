using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public class AnyUpdateHandler<T> : AbstractHandler<T> where T : class
    {
        private readonly Func<IContainer<T>, Task> _handleAsync;

        public AnyUpdateHandler(UpdateType updateType,
                                Func<Update, T?> getT,
                                Func<IContainer<T>, Task> callbak,
                                Filter<T>? filter,
                                int group) : base(updateType, getT, filter, group)
        {
            _handleAsync = callbak;
        }

        protected override IContainer<T> ContainerBuilder(
            IUpdater updater, Update update)
                => new AnyContainer<T>(GetT, updater, update);

        protected override async Task HandleAsync(IContainer<T> updateContainer)
        {
            await _handleAsync(updateContainer);
        }
    }
}
