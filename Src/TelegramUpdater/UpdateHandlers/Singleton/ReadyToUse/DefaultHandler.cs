using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Create update handler for any type of updates.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultHandler<T>
    : AbstractSingletonUpdateHandler<T, DefaultContainer<T>> where T : class
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
    /// <param name="endpoint">Determines if this is and endpoint handler.</param>
    /// <returns></returns>
    internal DefaultHandler(
        UpdateType updateType,
        Func<IContainer<T>, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filter = default,
        Func<Update, T?>? getT = default,
        bool endpoint = true) : base(updateType, getT, filter, endpoint)
        => _handleAsync = callback ??
            throw new ArgumentNullException(nameof(callback));

    internal override DefaultContainer<T> ContainerBuilder(HandlerInput input)
        => new(InnerUpdateExtractor, input, ExtraData);

    /// <inheritdoc/>
    protected override async Task HandleAsync(
        DefaultContainer<T> updateContainer)
    {
        await _handleAsync(updateContainer).ConfigureAwait(false);
    }
}
