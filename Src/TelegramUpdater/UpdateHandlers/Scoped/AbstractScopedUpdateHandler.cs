using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Abstract base for <see cref="IScopedUpdateHandler"/>s.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Create a new instance of <see cref="AbstractScopedUpdateHandler{T}"/>.
/// </remarks>
/// <param name="getT">Extract actual update from <see cref="Update"/>.</param>
/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractScopedUpdateHandler<T>(Func<Update, T?> getT)
    : AbstractHandlerProvider<T>, IScopedUpdateHandler where T : class
{
    private readonly Func<Update, T?> _getT = getT ?? throw new ArgumentNullException(nameof(getT));
    private IReadOnlyDictionary<string, object>? _extraData;

    IReadOnlyDictionary<string, object>? IScopedUpdateHandler.ExtraData
        => _extraData;

    internal IReadOnlyDictionary<string, object>? ExtraData
        => _extraData;

    /// <summary>
    /// Here you may handle the incoming update here.
    /// </summary>
    /// <param name="cntr">
    /// Provides everything you need and everything you want!
    /// </param>
    /// <returns></returns>
    protected abstract Task HandleAsync(IContainer<T> cntr);

    /// <inheritdoc/>
    async Task IUpdateHandler.HandleAsync(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        => await HandleAsync(ContainerBuilderWrapper(updater, shiningInfo)).ConfigureAwait(false);

    void IScopedUpdateHandler.SetExtraData(
        IReadOnlyDictionary<string, object>? extraData)
        => _extraData = extraData;

    /// <summary>
    /// A function to extract actual update from <see cref="Update"/>.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    internal protected T? GetT(Update update) => _getT(update);

    /// <summary>
    /// Create update container for this handler.
    /// </summary>
    internal protected abstract IContainer<T> ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo);

    private IContainer<T> ContainerBuilderWrapper(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo)
    {
        Container = ContainerBuilder(updater, shiningInfo);
        return Container;
    }

    /// <inheritdoc/>
    public override IContainer<T> Container { get; protected set; } = default!;
}
