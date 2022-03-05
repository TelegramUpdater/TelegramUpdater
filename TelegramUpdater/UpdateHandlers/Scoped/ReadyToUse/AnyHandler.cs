using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Creates an <see cref="IScopedUpdateHandler"/> for any type of update.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
public abstract class AnyHandler<T> : AbstractScopedUpdateHandler<T>
    where T : class
{
    internal AnyHandler(Func<Update, T?> getT, int group)
        : base(getT, group)
    {
    }

    /// <inheritdoc/>
    internal protected sealed override IContainer<T> ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo)
    {
        return new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);
    }
}
