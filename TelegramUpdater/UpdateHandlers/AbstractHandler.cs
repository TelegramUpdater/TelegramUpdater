﻿using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers
{
    public abstract class AbstractHandler<T> : ISingletonUpdateHandler where T : class
    {
        private readonly Func<Update, T?> _getT;
        private readonly Filter<T>? _filter;

        protected AbstractHandler(
            UpdateType updateType,
            Func<Update, T?> getT,
            Filter<T>? filter,
            int group)
        {
            _filter = filter;
            _getT = getT;
            UpdateType = updateType;
            Group = group;
        }

        public UpdateType UpdateType { get; }

        public int Group { get; }

        protected T? GetT(Update update) => _getT(update);

        // TODO: implement filter here.

        protected bool ShouldHandle(T t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        public async Task HandleAsync(IUpdater updater, Update update)
            => await HandleAsync(ContainerBuilder(updater, update));

        public bool ShouldHandle(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }

        protected abstract Task HandleAsync(IContainer<T> updateContainer);

        protected abstract IContainer<T> ContainerBuilder(IUpdater updater, Update update);
    }
}
