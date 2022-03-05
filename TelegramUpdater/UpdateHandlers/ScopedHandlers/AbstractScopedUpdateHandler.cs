using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Abstract base for <see cref="IScopedUpdateHandler"/>s.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractScopedUpdateHandler<T> : IScopedUpdateHandler
        where T : class
    {
        private readonly Func<Update, T?> _getT;
        private IReadOnlyDictionary<string, object>? _extraData;

        internal AbstractScopedUpdateHandler(Func<Update, T?> getT, int group)
        {
            Group = group;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
        }

        /// <inheritdoc/>
        public int Group { get; }

        IReadOnlyDictionary<string, object>? IScopedUpdateHandler.ExtraData
            => _extraData;

        internal IReadOnlyDictionary<string, object>? ExtraData
            => _extraData;

        /// <summary>
        /// Here you may handle the incoming update here.
        /// </summary>
        /// <param name="cntr">
        /// Provides everything you need and everything you want!
        /// </param>
        /// <returns></returns>
        protected abstract Task HandleAsync(IContainer<T> cntr);

        /// <inheritdoc/>
        async Task IUpdateHandler.HandleAsync(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo)
            => await HandleAsync(ContainerBuilder(updater, shiningInfo));

        void IScopedUpdateHandler.SetExtraData(
            IReadOnlyDictionary<string, object>? extraData)
            => _extraData = extraData;

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
