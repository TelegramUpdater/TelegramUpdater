using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    internal abstract class AbstractUpdateContainer<THandler, TUpdate> : IScopedHandlerContainer
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        private readonly Filter<TUpdate>? _filter;

        protected AbstractUpdateContainer(
            UpdateType updateType, Filter<TUpdate>? filter = default)
        {
            UpdateType = updateType;
            _filter = filter;
            ScopedHandlerType = typeof(THandler);
        }

        public Type ScopedHandlerType { get; }

        public UpdateType UpdateType { get; }

        protected bool ShouldHandle(TUpdate t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        protected abstract TUpdate? GetT(Update update);

        public bool ShouldHandle(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }
    }
}
