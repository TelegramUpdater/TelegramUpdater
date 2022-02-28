using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    /// <summary>
    /// Creates an <see cref="IScopedUpdateHandler"/> for any type of update.
    /// </summary>
    /// <typeparam name="T">Update type.</typeparam>
    public abstract class AnyScopedHandler<T> : AbstractScopedHandler<T> where T : class
    {
        internal AnyScopedHandler(Func<Update, T?> getT, int group) : base(getT, group)
        {
        }

        internal sealed override IContainer<T> ContainerBuilder(
            IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        {
            return new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);
        }
    }
}
