using TelegramUpdater.FilterAttributes;

namespace TelegramUpdater;

/// <summary>
/// Use this attribute to apply a filter on <see cref="UpdateHandlers.Scoped.IScopedUpdateHandler"/>
/// </summary>
/// <remarks>
/// The filter should have a parameterless constructor.
/// </remarks>
public sealed class ApplyFilterAttribute : AbstractFilterAttribute
{
    /// <summary>
    /// Type of filter to be applied.
    /// </summary>
    public Type FilterType { get; }

    /// <summary>
    /// Use this attribute to apply a filter on <see cref="UpdateHandlers.Scoped.IScopedUpdateHandler"/>
    /// </summary>
    /// <remarks>
    /// The filter should have a parameterless constructor.
    /// </remarks>
    /// <param name="filterType">Type of your filter which is a child class of <see cref="Filter{T}"/></param>
    public ApplyFilterAttribute(Type filterType)
    {
        FilterType = filterType;
    }

    /// <inheritdoc/>
    protected internal override object GetFilterTypeOf(Type requestedType)
    {
        var filter = Activator.CreateInstance(FilterType);
        if (filter == null)
            throw new InvalidOperationException($"Cannot initialize filter of type {FilterType}");
        return filter;
    }
}
