using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateChannels
{
    /// <summary>
    /// An abstract class for channel updates.
    /// </summary>
    /// <typeparam name="T">Type of update to channel</typeparam>
    public abstract class AbstractChannel<T> : IUpdateChannel where T : class
    {
        private readonly Func<Update, T?> _getT;
        private readonly Filter<T>? _filter;

        /// <summary>
        /// An abstract class for channel updates.
        /// </summary>
        /// <param name="updateType">Type of update.</param>
        /// <param name="getT">A function to select the right update from <see cref="Update"/></param>
        /// <param name="filter">Filter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected AbstractChannel(
            UpdateType updateType,
            Func<Update, T?> getT,
            Filter<T>? filter)
        {
            _filter = filter;
            UpdateType = updateType;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
        }

        /// <inheritdoc/>
        public UpdateType UpdateType { get; }

        /// <summary>
        /// A function to select the right update from <see cref="Update"/>
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public T? GetT(Update update) => _getT(update);

        /// <inheritdoc/>
        protected bool ShouldChannel(T t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        /// <summary>
        /// If this update should be channeled.
        /// </summary>
        public bool ShouldChannel(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldChannel(insider);
        }

        /// <inheritdoc/>
        internal abstract IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo);
    }
}
