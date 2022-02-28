using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Abstarct base for <see cref="IScopedUpdateHandler"/>s.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractScopedHandler<T> : IScopedUpdateHandler where T : class
    {
        private readonly Func<Update, T?> _getT;
        private IReadOnlyDictionary<string, object>? _extraData;

        internal AbstractScopedHandler(Func<Update, T?> getT, int group)
        {
            Group = group;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
        }

        /// <inheritdoc/>
        public int Group { get; }

        IReadOnlyDictionary<string, object>? IScopedUpdateHandler.ExtraData => _extraData;

        internal IReadOnlyDictionary<string, object>? ExtraData => _extraData;

        protected abstract Task HandleAsync(IContainer<T> updateContainer);

        internal abstract IContainer<T> ContainerBuilder(IUpdater updater, ShiningInfo<long, Update> shiningInfo);

        protected T? GetT(Update update) => _getT(update);

        /// <inheritdoc/>
        public async Task HandleAsync(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
            => await HandleAsync(ContainerBuilder(updater, shiningInfo));

        void IScopedUpdateHandler.SetExtraData(IReadOnlyDictionary<string, object>? extraData)
        {
            _extraData = extraData;
        }
    }
}
