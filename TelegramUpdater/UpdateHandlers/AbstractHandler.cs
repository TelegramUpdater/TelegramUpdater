using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers
{
    /// <summary>
    /// Abstract base to create update handlers.
    /// </summary>
    /// <typeparam name="T">Update type.</typeparam>
    public abstract class AbstractHandler<T> : ISingletonUpdateHandler
        where T : class
    {
        private readonly Func<Update, T?> _getT;
        private readonly IFilter<T>? _filter;

        internal AbstractHandler(
            UpdateType updateType,
            Func<Update, T?> getT,
            IFilter<T>? filter,
            int group)
        {
            if (updateType == UpdateType.Unknown)
                throw new ArgumentException(
                    $"There's nothing unknown here! {nameof(updateType)}");

            _filter = filter;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
            UpdateType = updateType;
            Group = group;
        }

        internal IReadOnlyDictionary<string, object>? ExtraData
            => _filter?.ExtraData;

        /// <inheritdoc/>
        public UpdateType UpdateType { get; }

        /// <inheritdoc/>
        public int Group { get; }

        /// <summary>
        /// Here you may handle the incoming update here.
        /// </summary>
        /// <param name="cntr">
        /// Provides everything you need and everything you want!
        /// </param>
        /// <returns></returns>
        protected abstract Task HandleAsync(IContainer<T> cntr);

        /// <summary>
        /// You can override this method instead of using filters.
        /// To apply a custom filter.
        /// </summary>
        /// <param name="input">Actual update.</param>
        /// <returns></returns>
        protected virtual bool ShouldHandle(T input)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(input);
        }

        /// <inheritdoc/>
        async Task IUpdateHandler.HandleAsync(IUpdater updater,
                                      ShiningInfo<long, Update> shiningInfo)
            => await HandleAsync(ContainerBuilder(updater, shiningInfo));

        /// <inheritdoc/>
        bool ISingletonUpdateHandler.ShouldHandle(Update update)
        {
            if (update.Type != UpdateType) return false;

            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }

        /// <summary>
        /// A function to extract actual update from <see cref="Update"/>.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        internal protected T? GetT(Update update) => _getT(update);

        internal abstract IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo);
    }
}
