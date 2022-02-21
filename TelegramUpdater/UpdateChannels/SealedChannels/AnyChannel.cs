using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    /// <summary>
    /// Create channel for any type of <see cref="Update"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AnyChannel<T> : AbstractChannel<T> where T : class
    {
        internal AnyChannel(
            UpdateType updateType, Func<Update, T?> getT, TimeSpan timeOut, Filter<T>? filter)
            : base(updateType, getT, timeOut, filter)
        { }

        internal override IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        {
            return new AnyContainer<T>(GetT, updater, shiningInfo);
        }
    }
}
