using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Abstarct base for <see cref="IScopedUpdateHandler"/> containers.
    /// </summary>
    /// <typeparam name="THandler">The handler, which is <see cref="IScopedUpdateHandler"/></typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    public abstract class AbstractUpdateContainer<THandler, TUpdate> : IScopedHandlerContainer
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        private readonly Filter<TUpdate>? _filter;

        internal AbstractUpdateContainer(
            UpdateType updateType, Filter<TUpdate>? filter = default)
        {
            UpdateType = updateType;
            _filter = filter;
            ScopedHandlerType = typeof(THandler);
        }

        /// <inheritdoc/>
        public Type ScopedHandlerType { get; }

        /// <inheritdoc/>
        public UpdateType UpdateType { get; }

        /// <summary>
        /// Checks if an update can be handled in a handler of type <see cref="ScopedHandlerType"/>.
        /// </summary>
        /// <param name="t">The inner update.</param>
        /// <returns></returns>
        internal bool ShouldHandle(TUpdate t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        /// <inheritdoc/>
        protected abstract TUpdate? GetT(Update update);

        /// <inheritdoc/>
        public bool ShouldHandle(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }
    }
}
