using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.AbstractHandlers
{
    /// <summary>
    /// A class to create update handler for any type of updates.
    /// </summary>
    /// <typeparam name="T"><see cref="Update"/> type</typeparam>
    public abstract class AnyUpdateHandlerAbs<T> : AbstractHandler<T>
        where T : class
    {
        /// <summary>
        /// Feed the base class to setup your update handler for any given
        /// type of <see cref="Update"/>.
        /// </summary>
        /// <param name="updateType">Type of update for this handler.</param>
        /// <param name="getT">
        /// A function to extract actual update from <see cref="Update"/>.
        /// </param>
        /// <param name="filter">Filters for this handler.</param>
        /// <param name="group">Handling priority for this handler.</param>
        public AnyUpdateHandlerAbs(UpdateType updateType,
                                   Func<Update, T?> getT,
                                   IFilter<T>? filter,
                                   int group)
            : base(updateType, getT, filter, group)
        { }

        internal override IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo)
            => new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);
    }
}
