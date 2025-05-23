﻿namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Abstract base for <see cref="IScopedUpdateHandler"/> containers.
/// </summary>
/// <typeparam name="THandler">
/// The handler, which is <see cref="IScopedUpdateHandler"/>
/// </typeparam>
/// <typeparam name="TUpdate">Update type.</typeparam>
public abstract class AbstractScopedUpdateHandlerContainer<THandler, TUpdate>
    : AbstractHandlerFiltering<TUpdate>, IGenericScopedUpdateHandlerContainer<TUpdate>
    where THandler : IScopedUpdateHandler
    where TUpdate : class
{
    /// <summary>
    /// Create a new instance of <see cref="AbstractScopedUpdateHandler{T, TContainer}"/>.
    /// </summary>
    /// <param name="updateType">Type of update.</param>
    /// <param name="filter">The filter.</param>
    /// <exception cref="ArgumentException"></exception>
    protected AbstractScopedUpdateHandlerContainer(
        UpdateType updateType, IFilter<UpdaterFilterInputs<TUpdate>>? filter = default)
    {
        if (updateType == UpdateType.Unknown)
            throw new ArgumentException(
                $"There's nothing unknown here! {nameof(updateType)}", nameof(updateType));

        UpdateType = updateType;
        ScopedHandlerType = typeof(THandler);

        Filter = filter;

        // Check for attributes
        Filter ??= ScopedHandlerType.GetFilterAttributes<UpdaterFilterInputs<TUpdate>>();
    }

    IReadOnlyDictionary<string, object>? IScopedUpdateHandlerContainer.ExtraData
        => Filter?.ExtraData;

    /// <inheritdoc/>
    public Type ScopedHandlerType { get; }

    /// <inheritdoc/>
    public override UpdateType UpdateType { get; }

    /// <inheritdoc/>
    public IFilter<UpdaterFilterInputs<TUpdate>>? Filter { get; }

    /// <summary>
    /// Checks if an update can be handled in a handler of type <see cref="ScopedHandlerType"/>.
    /// </summary>
    /// <returns></returns>
    protected override bool ShouldHandle(UpdaterFilterInputs<TUpdate> inputs)
    {
        if (Filter is null) return true;

        return Filter.TheyShellPass(inputs);
    }

    /// <summary>
    /// A function to extract actual update from <see cref="Update"/>.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    internal protected abstract TUpdate? ExtractInnerUpdate(Update update);

    /// <inheritdoc/>
    protected override Func<Update, TUpdate?> InnerUpdateExtractor => ExtractInnerUpdate;
}
