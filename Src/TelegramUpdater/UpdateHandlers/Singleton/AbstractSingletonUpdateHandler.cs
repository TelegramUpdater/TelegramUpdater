using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton;

/// <summary>
/// Abstract base to create update handlers.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
/// <typeparam name="TContainer">Type of the container.</typeparam>
public abstract class AbstractSingletonUpdateHandler<T, TContainer>
    : AbstractHandlerProvider<T>, IGenericSingletonUpdateHandler<T>
    where T : class
    where TContainer : IContainer<T>
{
    /// <summary>
    /// Create a new instance of <see cref="AbstractSingletonUpdateHandler{T, TContainer}"/>
    /// </summary>
    /// <param name="updateType">Target update type.</param>
    /// <param name="getT">To extract this update from <see cref="Update"/></param>
    /// <param name="filter">Filters.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected AbstractSingletonUpdateHandler(
        UpdateType updateType,
        Func<Update, T?>? getT = default,
        IFilter<UpdaterFilterInputs<T>>? filter = default)
    {
        if (updateType == UpdateType.Unknown)
            throw new ArgumentException(
                $"There's nothing unknown here! {nameof(updateType)}", nameof(updateType));

        Filter = filter;
        GetActualUpdate = getT ?? throw new ArgumentNullException(nameof(getT));
        UpdateType = updateType;
    }

    internal IReadOnlyDictionary<string, object>? ExtraData
        => Filter?.ExtraData;

    /// <inheritdoc/>
    public IFilter<UpdaterFilterInputs<T>>? Filter { get; }

    /// <inheritdoc/>
    public Func<Update, T?>? GetActualUpdate { get; }

    /// <summary>
    /// Resolve inner update from <see cref="Update"/> using <see cref="GetActualUpdate"/> if not null or
    /// using <see cref="UpdaterExtensions.GetInnerUpdate{T}(Update)"/>
    /// </summary>
    internal protected Func<Update, T?> ExtractInnerUpdater
        => GetActualUpdate ?? ((update) => update.GetInnerUpdate<T>());

    /// <inheritdoc />
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Here you may handle the incoming update.
    /// </summary>
    /// <param name="container">
    /// Provides everything you need and everything you want!
    /// </param>
    /// <returns></returns>
    protected abstract Task HandleAsync(TContainer container);

    /// <summary>
    /// You can override this method instead of using filters.
    /// To apply a custom filter.
    /// </summary>
    /// <returns></returns>
    protected virtual bool ShouldHandle(UpdaterFilterInputs<T> inputs)
    {
        if (Filter is null) return true;

        return Filter.TheyShellPass(inputs);
    }

    /// <inheritdoc/>
    async Task IUpdateHandler.HandleAsync(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
    {
        var container = ContainerBuilder(updater, shiningInfo);
        Container = container;
        await HandleAsync(container).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public bool ShouldHandle(UpdaterFilterInputs<Update> inputs)
    {
        if (inputs.Input.Type != UpdateType) return false;

        var insider = ExtractInnerUpdater(inputs.Input);

        if (insider == null) return false;

        return ShouldHandle(new UpdaterFilterInputs<T>(inputs.Updater, insider));
    }

    internal abstract TContainer ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo);

    /// <inheritdoc/>
    public override IContainer<T> Container { get; protected set; } = default!;
}
