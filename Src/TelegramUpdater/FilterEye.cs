namespace TelegramUpdater;

/// <summary>
/// Information about an evaluated filter.
/// </summary>
public class FilterEyeInformation
{
    /// <summary>
    /// Indicates the result of an already evaluated filter.
    /// </summary>
    public bool Passed { get; }

    /// <summary>
    /// Extra data for an already evaluated filter.
    /// </summary>
    public IDictionary<string, object>? ExtraData { get; }

    /// <param name="passed"></param>
    /// <param name="extraData"></param>
    internal FilterEyeInformation(bool passed, IDictionary<string, object>? extraData)
    {
        Passed = passed;
        ExtraData = extraData;
    }
}

/// <summary>
/// This is the filter eye, responsible for watching over filters to ensure not a single filter
/// evaluates more than once if the context is persistence.
/// </summary>
/// <remarks>
/// The filter's <see cref="IFilter{T}.ExtraData"/> will also be cached after first evaluation.
/// </remarks>
public class FilterEye
{
    /// <summary>
    /// Creates a new instance of <see cref="FilterEye"/>.
    /// </summary>
    public FilterEye()
    {
        CachedResults = new Dictionary<Type, FilterEyeInformation>();
    }

    /// <summary>
    /// Cached results of previous evaluations for each filter
    /// </summary>
    public IDictionary<Type, FilterEyeInformation> CachedResults { get; }
}
