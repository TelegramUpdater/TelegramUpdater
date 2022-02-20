using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    /// <summary>
    /// Create update handler for any type of updates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        internal override IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo)
                => new AnyContainer<T>(GetT, updater, shiningInfo);

        /// <inheritdoc/>
        protected override async Task HandleAsync(IContainer<T> updateContainer)
        {
            await _handleAsync(updateContainer);
        }
    }
}
