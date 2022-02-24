using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers
{
    /// <summary>
    /// Abstract base to create update handlers.
    /// </summary>
    /// <typeparam name="T">Update type.</typeparam>
    public abstract class AbstractHandler<T> : ISingletonUpdateHandler where T : class
    {
        private readonly Func<Update, T?> _getT;
        private readonly Filter<T>? _filter;

        internal AbstractHandler(
            UpdateType updateType,
            Func<Update, T?> getT,
            Filter<T>? filter,
            int group)
        {
            if (updateType == UpdateType.Unknown)
                throw new ArgumentException($"There's nothing uknown here! {nameof(updateType)}");

            _filter = filter;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
            UpdateType = updateType;
            Group = group;
        }

        /// <inheritdoc/>
        public UpdateType UpdateType { get; }

        /// <inheritdoc/>
        public int Group { get; }

        protected T? GetT(Update update) => _getT(update);

        // TODO: implement filter here.

        protected bool ShouldHandle(T t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        /// <inheritdoc/>
        public async Task HandleAsync(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
            => await HandleAsync(ContainerBuilder(updater, shiningInfo));

        /// <inheritdoc/>
        public bool ShouldHandle(Update update)
        {
            if (update.Type != UpdateType) return false;

            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }

        /// <inheritdoc/>
        protected abstract Task HandleAsync(IContainer<T> updateContainer);

        internal abstract IContainer<T> ContainerBuilder(IUpdater updater, ShiningInfo<long, Update> shiningInfo);
    }
}
