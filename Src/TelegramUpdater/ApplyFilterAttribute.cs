using TelegramUpdater.FilterAttributes;

namespace TelegramUpdater;

/// <summary>
/// Use this attribute to apply a filter on <see cref="UpdateHandlers.Scoped.IScopedUpdateHandler"/>
/// </summary>
/// <remarks>
/// The filter should have a parameterless constructor.
/// </remarks>
/// <remarks>
/// Use this attribute to apply a filter on <see cref="UpdateHandlers.Scoped.IScopedUpdateHandler"/>
/// </remarks>
/// <remarks>
/// The filter should have a parameterless constructor.
/// </remarks>
/// <param name="filterType">Type of your filter which is a child class of <see cref="Filter{T}"/></param>
public sealed class ApplyFilterAttribute(Type filterType)
    : AbstractFilterAttribute
{
    /// <summary>
    /// Type of filter to be applied.
    /// </summary>
    public Type FilterType { get; } = filterType;

    /// <inheritdoc/>
    protected internal override object GetFilterTypeOf(Type requestedType)
    {
        var filter = Activator.CreateInstance(FilterType);
        return filter ?? throw new InvalidOperationException(
                $"Cannot initialize filter of type {FilterType}");
    }
}
