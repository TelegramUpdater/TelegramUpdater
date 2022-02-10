using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateChannels.AbstractChannels
{
    public class AnyChannelAbs<T> : AbstractChannel<T> where T : class
    {
        public AnyChannelAbs(
            UpdateType updateType, Func<Update, T?> getT, Filter<T>? filter)
            : base(updateType, getT, filter)
        {
        }

        protected override IContainer<T> ContainerBuilder(
            IUpdater updater, Update update)
        {
            return new AnyContainer<T>(GetT, updater, update);
        }
    }
}
