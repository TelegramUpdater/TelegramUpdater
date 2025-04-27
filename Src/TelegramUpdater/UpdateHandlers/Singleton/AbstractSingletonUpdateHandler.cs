using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton;

/// <summary>
/// Abstract base to create update handlers.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
public abstract class AbstractSingletonUpdateHandler<T>
    : IGenericSingletonUpdateHandler<T>
    where T : class
{
    // TODO: use internal protected for GetT like scoped.

    /// <summary>
    /// Create a new instance of <see cref="AbstractSingletonUpdateHandler{T}"/>
    /// </summary>
    /// <param name="updateType">Target update type.</param>
    /// <param name="getT">To extract this update from <see cref="Update"/></param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">Priority of handling.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected AbstractSingletonUpdateHandler(
        UpdateType updateType,
        Func<Update, T?> getT,
        IFilter<UpdaterFilterInputs<T>>? filter,
        int group)
    {
        if (updateType == UpdateType.Unknown)
            throw new ArgumentException(
                $"There's nothing unknown here! {nameof(updateType)}", nameof(updateType));

        Filter = filter;
        GetActualUpdate = getT ?? throw new ArgumentNullException(nameof(getT));
        UpdateType = updateType;
        Group = group;
    }

    internal IReadOnlyDictionary<string, object>? ExtraData
        => Filter?.ExtraData;

    /// <inheritdoc/>
    public UpdateType UpdateType { get; }

    /// <inheritdoc/>
    public int Group { get; }

    /// <inheritdoc/>
    public IFilter<UpdaterFilterInputs<T>>? Filter { get; }

    /// <inheritdoc/>
    public Func<Update, T?> GetActualUpdate { get; }

    /// <summary>
    /// Here you may handle the incoming update here.
    /// </summary>
    /// <param name="cntr">
    /// Provides everything you need and everything you want!
    /// </param>
    /// <returns></returns>
    protected abstract Task HandleAsync(IContainer<T> cntr);

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
    async Task IUpdateHandler.HandleAsync(IUpdater updater,
                                  ShiningInfo<long, Update> shiningInfo)
        => await HandleAsync(ContainerBuilder(updater, shiningInfo)).ConfigureAwait(false);

    /// <inheritdoc/>
    public bool ShouldHandle(UpdaterFilterInputs<Update> inputs)
    {
        if (inputs.Input.Type != UpdateType) return false;

        var insider = GetT(inputs.Input);

        if (insider == null) return false;

        return ShouldHandle(new UpdaterFilterInputs<T>(inputs.Updater, insider));
    }

    /// <summary>
    /// A function to extract actual update from <see cref="Update"/>.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    internal protected T? GetT(Update update) => GetActualUpdate(update);

    internal abstract IContainer<T> ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo);
}
