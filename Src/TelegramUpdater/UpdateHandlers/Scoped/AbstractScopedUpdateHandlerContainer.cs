namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Abstract base for <see cref="IScopedUpdateHandler"/> containers.
/// </summary>
/// <typeparam name="THandler">
/// The handler, which is <see cref="IScopedUpdateHandler"/>
/// </typeparam>
/// <typeparam name="TUpdate">Update type.</typeparam>
public abstract class AbstractScopedUpdateHandlerContainer<THandler, TUpdate>
    : IGenericScopedUpdateHandlerContainer<TUpdate>
    where THandler : IScopedUpdateHandler
    where TUpdate : class
{
    internal AbstractScopedUpdateHandlerContainer(
        UpdateType updateType, IFilter<TUpdate>? filter = default)
    {
        if (updateType == UpdateType.Unknown)
            throw new ArgumentException(
                $"There's nothing unknown here! {nameof(updateType)}");

        UpdateType = updateType;
        ScopedHandlerType = typeof(THandler);

        Filter = filter;

        if (Filter == null)
        {
            // Check for attributes
            Filter = ScopedHandlerType.GetFilterAttributes<TUpdate>();
        }
    }

    IReadOnlyDictionary<string, object>? IScopedUpdateHandlerContainer.ExtraData
        => Filter?.ExtraData;

    /// <inheritdoc/>
    public Type ScopedHandlerType { get; }

    /// <inheritdoc/>
    public UpdateType UpdateType { get; }

    /// <inheritdoc/>
    public IFilter<TUpdate>? Filter { get; }

    /// <summary>
    /// Checks if an update can be handled in a handler of type <see cref="ScopedHandlerType"/>.
    /// </summary>
    /// <param name="t">The inner update.</param>
    /// <param name="updater">The updater instance.</param>
    /// <returns></returns>
    private bool ShouldHandle(IUpdater updater, TUpdate t)
    {
        if (Filter is null) return true;

        return Filter.TheyShellPass(updater, t);
    }

    /// <summary>
    /// A function to extract actual update from <see cref="Update"/>.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    internal protected abstract TUpdate? GetT(Update update);

    /// <inheritdoc/>
    public bool ShouldHandle(IUpdater updater, Update update)
    {
        if (update.Type != UpdateType) return false;

        var insider = GetT(update);

        if (insider == null) return false;

        return ShouldHandle(updater, insider);
    }
}
