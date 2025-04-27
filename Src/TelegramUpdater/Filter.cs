using System.Reflection;
using TelegramUpdater.FilterAttributes;

namespace TelegramUpdater;

/// <summary>
/// Base interface for filters.
/// </summary>
/// <typeparam name="T">A type that filter will apply to.</typeparam>
public interface IFilter<T>
{
    /// <summary>
    /// Indicates if an input of type <typeparamref name="T"/> can pass this filter
    /// </summary>
    /// <param name="input">The input value to check</param>
    /// <param name="eye">The eye!</param>
    /// <returns></returns>
    public bool Evaluate(T input, FilterEye? eye = default);

    /// <summary>
    /// A dictionary of extra data produced by this filter.
    /// </summary>
    public IDictionary<string, object>? ExtraData { get; protected set; }

    /// <summary>
    /// Indicates if this filter can be used along with <see cref="FilterEye"/>.
    /// </summary>
    public bool EyeAssistant => false;

    /// <summary>
    /// Returns an <see cref="AndFilter{T}"/> version.
    /// <para>
    /// This and <paramref name="simpleFilter"/>
    /// </para>
    /// </summary>
    public IFilter<T> And(IFilter<T> simpleFilter)
        => new AndFilter<T>(this, simpleFilter);

    /// <summary>
    /// Returns an <see cref="OrFilter{T}"/> version.
    /// <para>
    /// This Or <paramref name="simpleFilter"/>
    /// </para>
    /// </summary>
    public IFilter<T> Or(IFilter<T> simpleFilter)
        => new OrFilter<T>(this, simpleFilter);

    /// <summary>
    /// Returns a reversed version of this <see cref="Filter{T}"/>
    /// </summary>
    /// <returns></returns>
    public IFilter<T> Reverse() => new ReverseFilter<T>(this);

    /// <summary>
    /// Creates an <see cref="AndFilter{T}"/>
    /// </summary>
    public static IFilter<T> operator &(IFilter<T> a, IFilter<T> b)
        => new AndFilter<T>(a, b);

    /// <summary>
    /// Creates an <see cref="OrFilter{T}"/>
    /// </summary>
    public static IFilter<T> operator |(IFilter<T> a, IFilter<T> b)
        => new OrFilter<T>(a, b);

    /// <summary>
    /// Creates a reversed version of this <see cref="Filter{T}"/>
    /// </summary>
    public static IFilter<T> operator ~(IFilter<T> a)
        => new ReverseFilter<T>(a);

    /// <summary>
    /// Evaluate a filter using <see cref="FilterEye"/>.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="eye"></param>
    /// <param name="rawEvaluate"></param>
    /// <returns></returns>
    public bool EvaluateWithEye(
        T input,
        FilterEye? eye,
        Func<T, bool> rawEvaluate)
    {
        if (EyeAssistant && eye is not null)
        {
            var filterType = GetType();
            if (eye.CachedResults.TryGetValue(filterType, out var info))
            {
                ExtraData = info.ExtraData;
                return info.Passed;
            }

            var result = rawEvaluate(input);
            eye.CachedResults.Add(filterType, new(result, ExtraData));
            return result;
        }

        return rawEvaluate(input);
    }
}

/// <summary>
/// Interface for joined filters.
/// </summary>
/// <typeparam name="T">A type that filter will apply to.</typeparam>
public interface IJoinedFilter<T> : IFilter<T>
{
    /// <summary>
    /// Filters that are going to join.
    /// </summary>
    public IFilter<T>[] Filters { get; }
}

/// <summary>
/// A simple basic filter
/// </summary>
/// <typeparam name="T">Object type that filter is gonna apply to</typeparam>
/// <remarks>
/// Creates a simple basic filter
/// </remarks>
/// <param name="filter">A function to check the input and return a boolean</param>
/// <param name="eyeAssistant"></param>
public class Filter<T>(Func<T, bool>? filter = default, bool eyeAssistant = false) : IFilter<T>
{
    private readonly Func<T, bool>? _filter = filter;

    /// <inheritdoc/>
    public virtual IDictionary<string, object>? ExtraData { get; set; }

    /// <inheritdoc/>
    public virtual bool EyeAssistant { get; } = eyeAssistant;

    internal void AddOrUpdateData(string key, object value)
    {
        ExtraData ??= new Dictionary<string, object>(StringComparer.Ordinal);

        if (!ExtraData.TryAdd(key, value))
            ExtraData[key] = value;
    }

    /// <inheritdoc/>
    public bool Evaluate(T input, FilterEye? eye = default)
    {
        return (this as IFilter<T>).EvaluateWithEye(input, eye, TheyShellPass);
    }

    /// <inheritdoc/>
    protected virtual bool TheyShellPass(T input)
        => input != null && (_filter is null || _filter(input));

    /// <summary>
    /// Converts a <paramref name="filter"/> to <see cref="Filter{T}"/>
    /// </summary>
    /// <param name="filter"></param>

    public static implicit operator Filter<T>(Func<T, bool> filter)
        => new(filter);

    /// <summary>
    /// Converts a filter to a function.
    /// </summary>
    /// <param name="filter"></param>
    public static implicit operator Func<T, bool>(Filter<T> filter)
        => filter.TheyShellPass;

    /// <summary>
    /// Returns an <see cref="AndFilter{T}"/> version.
    /// <para>
    /// This and <paramref name="simpleFilter"/>
    /// </para>
    /// </summary>
    public IFilter<T> And(IFilter<T> simpleFilter)
        => new AndFilter<T>(this, simpleFilter);

    /// <summary>
    /// Returns an <see cref="OrFilter{T}"/> version.
    /// <para>
    /// This Or <paramref name="simpleFilter"/>
    /// </para>
    /// </summary>
    public IFilter<T> Or(IFilter<T> simpleFilter)
        => new OrFilter<T>(this, simpleFilter);

    /// <summary>
    /// Returns a reversed version of this <see cref="Filter{T}"/>
    /// </summary>
    /// <returns></returns>
    public IFilter<T> Reverse() => new ReverseFilter<T>(this);

    /// <summary>
    /// Creates an <see cref="AndFilter{T}"/>
    /// </summary>
    public static IFilter<T> operator &(Filter<T> a, Filter<T> b)
        => new AndFilter<T>(a, b);

    /// <summary>
    /// Creates an <see cref="OrFilter{T}"/>
    /// </summary>
    public static IFilter<T> operator |(Filter<T> a, Filter<T> b)
        => new OrFilter<T>(a, b);

    /// <summary>
    /// Creates a reversed version of this <see cref="Filter{T}"/>
    /// </summary>
    public static IFilter<T> operator ~(Filter<T> a)
        => new ReverseFilter<T>(a);
}

/// <summary>
/// Creates a simple reverse filter
/// </summary>
/// <remarks>
/// Creates a reverse filter ( Like not filter ), use not operator
/// </remarks>
public class ReverseFilter<T>(IFilter<T> filter1) : IFilter<T>
//: Filter<T>((x) => !filter1.Evaluate(x))
{
    /// <inheritdoc/>
    public IDictionary<string, object>? ExtraData { get; set; }

    /// <inheritdoc/>
    public bool Evaluate(T input, FilterEye? eye = null) => !filter1.Evaluate(input, eye);
}

/// <summary>
/// Used when two filters are going to join other.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class JoinedFilter<T>(params IFilter<T>[] filters)
    : IJoinedFilter<T>
{
    /// <inheritdoc/>
    public IFilter<T>[] Filters { get; } = filters;

    /// <inheritdoc/>
    public IDictionary<string, object>? ExtraData { get; set; }

    /// <summary>
    /// Check if the filters are passed here.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="eye">The eye!</param>
    /// <returns></returns>
    protected abstract bool InnerTheyShellPass(T input, FilterEye? eye);

    private bool TheyShellPass(T input, FilterEye? eye = default)
    {
        var shellPass = InnerTheyShellPass(input, eye);
        ExtraData = Filters.Where(x => x.ExtraData is not null)
            .SelectMany(x => x.ExtraData!) // extra data not null here.
#if NET8_0_OR_GREATER
            .DistinctBy(x => x.Key) // Is it required ?
#else
            .GroupBy(x => x.Key, StringComparer.Ordinal).Select(x => x.First())
#endif
            .ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        return shellPass;
    }

    bool IFilter<T>.Evaluate(T input, FilterEye? eye) => TheyShellPass(input, eye);
}

/// <summary>
/// Creates a simple and filter
/// </summary>
/// <remarks>
/// Creates a and filter ( Like filter1 and filter2 ), use and operator
/// </remarks>
public class AndFilter<T>(IFilter<T> filter1, IFilter<T> filter2)
    : JoinedFilter<T>(filter1, filter2)
{
    /// <inheritdoc/>
    protected override bool InnerTheyShellPass(T input, FilterEye? eye)
    {
        return Filters[0].Evaluate(input, eye) &&
            Filters[1].Evaluate(input, eye);
    }
}

/// <summary>
/// Creates a simple or filter
/// </summary>
/// <remarks>
/// Creates an or filter ( Like filter1 or filter2 ), use or operator
/// </remarks>
public class OrFilter<T>(IFilter<T> filter1, IFilter<T> filter2)
    : JoinedFilter<T>(filter1, filter2)
{
    /// <inheritdoc/>
    protected override bool InnerTheyShellPass(T input, FilterEye? eye)
    {
        return Filters[0].Evaluate(input, eye) ||
            Filters[1].Evaluate(input, eye);
    }
}

/// <summary>
/// Inputs for a filter inside <see cref="IUpdater"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="updater"></param>
/// <param name="input"></param>
public class UpdaterFilterInputs<T>(IUpdater updater, T input)
{
    /// <summary>
    /// The updater.
    /// </summary>
    public IUpdater Updater { get; } = updater;

    /// <summary>
    /// The input.
    /// </summary>
    public T Input { get; } = input;
}

/// <summary>
/// A filter specialized to be used with <see cref="IUpdater"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class UpdaterFilter<T> : Filter<UpdaterFilterInputs<T>>
{
    /// <summary>
    /// Create a new instance of <see cref="UpdaterFilter{T}"/>
    /// </summary>
    public UpdaterFilter(bool eyeAssistant = false) : base(eyeAssistant: eyeAssistant)
    {
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterFilter{T}"/>
    /// </summary>
    public UpdaterFilter(Func<UpdaterFilterInputs<T>, bool> filter, bool eyeAssistant = false)
        : base(filter, eyeAssistant)
    {
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterFilter{T}"/>
    /// </summary>
    public UpdaterFilter(Func<T, bool> filter, bool eyeAssistant = false)
        : base((input) => filter(input.Input), eyeAssistant)
    {
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterFilter{T}"/>
    /// </summary>
    public UpdaterFilter(Func<IUpdater, T, bool> filter, bool eyeAssistant = false)
        : base((input) => filter(input.Updater, input.Input), eyeAssistant)
    {
    }
}

/// <summary>
/// Extension methods for filters
/// </summary>
public static class FiltersExtensions
{
    /// <summary>
    /// Checks if <paramref name="type"/> is an filter.
    /// </summary>
    /// <param name="type">Type of your filter.</param>
    /// <returns></returns>
    public static bool IsFilter(this Type type)
    {
        return type.GetInterfaces().Any(x =>
          x.IsGenericType &&
          x.GetGenericTypeDefinition() == typeof(IFilter<>));
    }

    /// <summary>
    /// Checks if <paramref name="filterType"/> is an filter for <paramref name="genericType"/>.
    /// </summary>
    /// <param name="filterType">Type of your filter.</param>
    /// <param name="genericType">The type which filter applied on.</param>
    public static bool IsFilterOfType(this Type filterType, Type genericType)
    {
        return filterType.GetInterfaces().Any(x =>
        {
            if (x.IsGenericType)
            {
                var f = x.GetGenericTypeDefinition();
                if (f == typeof(IFilter<>))
                {
                    return x.GetGenericArguments()[0] == genericType;
                }
            }

            return false;
        });
    }

    /// <summary>
    /// Checks if <paramref name="filterType"/> is an filter for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type which filter applied on.</typeparam>
    /// <param name="filterType">Type of your filter.</param>
    public static bool IsFilterOfType<T>(this Type filterType)
    {
        return filterType.GetInterfaces().Any(x =>
        {
            if (x.IsGenericType)
            {
                var f = x.GetGenericTypeDefinition();
                if (f == typeof(IFilter<>))
                {
                    return x.GetGenericArguments()[0] == typeof(T);
                }
            }

            return false;
        });
    }

    /// <summary>
    /// Discover all sub filters of this filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static IEnumerable<IFilter<T>> DiscoverNestedFilters<T>(this IFilter<T> filter)
    {
        if (filter is IJoinedFilter<T> joinedFilter)
        {
            foreach (var parts in joinedFilter.Filters)
            {
                foreach (var nestedNested in parts.DiscoverNestedFilters())
                {
                    yield return nestedNested;
                }
            }
        }
        else
        {
            yield return filter;
        }
    }

    private static List<List<AbstractFilterAttribute>> BatchifyFilterAttributes(
        this IEnumerable<AbstractFilterAttribute> filterAttributes)
    {
        var batches = new List<List<AbstractFilterAttribute>>
        {
            new(),
        };

        foreach (var filterAttribute in filterAttributes)
        {
            if ((filterAttribute.BatchStart && batches[^1].Count == 0) ||
                !filterAttribute.BatchStart)
            {
                batches[^1].Add(filterAttribute);
            }
            else
            {
                batches.Add([filterAttribute]);
            }
        }

        return batches;
    }

    private static IFilter<T> AppendFilter<T>(
        this IFilter<T>? cur, IFilter<T> toAppend, bool asOr, bool reverse)
    {
        if (reverse)
        {
            toAppend = ~toAppend; // Reverse the filter.
        }

        if (cur == null)
        {
            cur = toAppend;
        }
        else
        {
            if (asOr)
            {
                cur |= toAppend;
            }
            else
            {
                cur &= toAppend;
            }
        }

        return cur;
    }

    private static (IFilter<T>? filter, bool asOr, bool reverse) JoinFilterAttributes<T>(
        this IEnumerable<AbstractFilterAttribute> filterAttributes)
    {
        if (!filterAttributes.Any()) return (null, false, false);

        IFilter<T> filter = null!;
        foreach (var item in filterAttributes)
        {
            var f = (IFilter<T>?)item.GetFilterTypeOf(typeof(T));
            if (f != null)
            {
                filter = filter.AppendFilter(f, item.ApplyAsOr, item.Reverse);
            }
        }

        var batchStart = filterAttributes.ElementAt(0);
        return (filter, batchStart.OrBatch, batchStart.ReverseBatch);
    }

    private static IFilter<T>? JoinFilterAttributes<T>(
        this IEnumerable<IEnumerable<AbstractFilterAttribute>> filterAttributes)
    {
        IFilter<T>? filter = null;
        foreach (var batch in filterAttributes)
        {
            (IFilter<T>? f, bool asOr, bool reverse) = batch.JoinFilterAttributes<T>(); 

            if (f is not null)
            {
                filter = filter.AppendFilter(
                    f, asOr, reverse);
            }
        }

        return filter;
    }

    internal static IFilter<T>? GetFilterAttributes<T>(this Type type)
    {
        var applied = type.GetCustomAttributes<AbstractFilterAttribute>();
        var batches = applied.BatchifyFilterAttributes();
        return batches.JoinFilterAttributes<T>();
    }

    internal static IFilter<T>? GetFilterAttributes<T>(this MethodInfo method)
    {
        var applied = method.GetCustomAttributes<AbstractFilterAttribute>();
        var batches = applied.BatchifyFilterAttributes();
        return batches.JoinFilterAttributes<T>();
    }
}

