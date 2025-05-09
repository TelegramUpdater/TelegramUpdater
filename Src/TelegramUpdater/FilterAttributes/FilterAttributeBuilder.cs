namespace TelegramUpdater.FilterAttributes;

/// <summary>
/// Using this class you can build filter attributes which can be cross update type.
/// </summary>
public abstract class FilterAttributeBuilder : AbstractUpdaterFilterAttribute
{
    private readonly Dictionary<Type, object> _filtersPerUpdateType;

    /// <summary>
    /// Init a new instance of <see cref="FilterAttributeBuilder"/>.
    /// </summary>
    /// <remarks>
    /// User <see cref="AddFilterForUpdate{T}(UpdaterFilter{T})"/>
    /// to add filters for different type of updates.
    /// </remarks>
    protected FilterAttributeBuilder(Action<FilterAttributeBuilder> builder)
    {
        _filtersPerUpdateType = [];

        builder(this);
    }

    /// <summary>
    /// Add a filter for another type of update <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="Message"/>, <see cref="CallbackQuery"/> and more ...
    /// </remarks>
    /// <typeparam name="T">Type of update.</typeparam>
    /// <param name="filter">Filter to apply on updates of type <typeparamref name="T"/>.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public FilterAttributeBuilder AddFilterForUpdate<T>(UpdaterFilter<T> filter) where T : class
    {
        _filtersPerUpdateType.Add(typeof(T), filter);
        return this;
    }

    /// <inheritdoc/>
    protected internal override object GetUpdaterFilterTypeOf(Type requestedType)
    {
        if (!_filtersPerUpdateType.TryGetValue(requestedType, out object? value))
        {
            throw new InvalidOperationException($"{GetType()} dose not support {requestedType}");
        }

        return value;
    }
}
