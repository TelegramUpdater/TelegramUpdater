using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.AbstractHandlers
{
    /// <summary>
    /// A class to create update handler for any type of updates.
    /// </summary>
    /// <typeparam name="T"><see cref="Update"/> type</typeparam>
    public abstract class AnyUpdateHandlerAbs<T> : AbstractHandler<T> where T : class
    {
        public AnyUpdateHandlerAbs(
            UpdateType updateType, Func<Update, T?> getT, IFilter<T>? filter, int group)
            : base(updateType, getT, filter, group)
        {
        }

        internal override IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        {
            return new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);
        }
    }
}
