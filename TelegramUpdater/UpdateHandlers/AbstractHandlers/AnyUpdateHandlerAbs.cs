﻿using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.AbstractHandlers
{
    public abstract class AnyUpdateHandlerAbs<T> : AbstractHandler<T> where T : class
    {
        protected AnyUpdateHandlerAbs(
            UpdateType updateType, Func<Update, T?> getT, Filter<T>? filter, int group)
            : base(updateType, getT, filter, group)
        {
        }

        protected override IContainer<T> ContainerBuilder(
            IUpdater updater, Update update)
        {
            return new AnyContainer<T>(GetT, updater, update);
        }
    }
}
