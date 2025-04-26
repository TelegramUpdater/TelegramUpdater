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
/// <param name="group">Handling priority.</param>
/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractScopedUpdateHandler<T>(Func<Update, T?> getT, int group) : IScopedUpdateHandler
    where T : class
{
    private readonly Func<Update, T?> _getT = getT ?? throw new ArgumentNullException(nameof(getT));
    private IReadOnlyDictionary<string, object>? _extraData;
    private IContainer<T> _container = default!;

    /// <inheritdoc/>
    public int Group { get; } = group;

    /// <summary>
    /// Bot client instance.
    /// </summary>
    public ITelegramBotClient BotClient => _container.BotClient;

    /// <summary>
    /// The updater instance.
    /// </summary>
    public IUpdater Updater => _container.Updater;

    /// <summary>
    /// The actual update. one of <see cref="Update"/> properties.
    /// </summary>
    public T ActualUpdate => _container.Update;

    /// <summary>
    /// Container itself. same as container in <see cref="HandleAsync(IContainer{T})"/>.
    /// </summary>
    public IContainer<T> Container => _container;

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
        _container = ContainerBuilder(updater, shiningInfo);
        return _container;
    }
}
