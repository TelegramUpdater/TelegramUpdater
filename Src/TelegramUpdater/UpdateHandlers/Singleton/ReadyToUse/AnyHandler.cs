using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Create update handler for any type of updates.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AnyHandler<T> : AbstractSingletonUpdateHandler<T> where T : class
{
    private readonly Func<IContainer<T>, Task> _handleAsync;

    /// <summary>
    /// Create an update handler for any given type of update.
    /// </summary>
    /// <param name="updateType">Type of update for this handler.</param>
    /// <param name="getT">
    /// A function to extract actual update from <see cref="Update"/>.
    /// </param>
    /// <param name="filter">Filters for this handler.</param>
    /// <param name="callback">
    /// A callback function where you may handle the incoming update.
    /// </param>
    /// <returns></returns>
    internal AnyHandler(
        UpdateType updateType,
        Func<Update, T?> getT,
        Func<IContainer<T>, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filter) : base(updateType, getT, filter)
    {
        _handleAsync = callback ??
            throw new ArgumentNullException(nameof(callback));
    }

    internal override IContainer<T> ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        => new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);

    /// <inheritdoc/>
    protected override async Task HandleAsync(
        IContainer<T> updateContainer)
    {
        await _handleAsync(updateContainer).ConfigureAwait(false);
    }
}
