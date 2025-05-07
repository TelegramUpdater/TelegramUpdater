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
    /// <returns></returns>
    public bool TheyShellPass(T input);

    /// <summary>
    /// A dictionary of extra data produced by this filter.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ExtraData { get; }

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
public class Filter<T>(Func<T, bool>? filter = default) : IFilter<T>
{
    private readonly Func<T, bool>? _filter = filter;
    private Dictionary<string, object>? _extraData;

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, object>? ExtraData => _extraData;

    internal void AddOrUpdateData(string key, object value)
    {
        _extraData ??= new Dictionary<string, object>(StringComparer.Ordinal);

        if (!_extraData.TryAdd(key, value))
            _extraData[key] = value;
    }

    /// <inheritdoc/>
    public virtual bool TheyShellPass(T input)
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
    public static Filter<T> operator ~(Filter<T> a)
        => new ReverseFilter<T>(a);
}

/// <summary>
/// Creates a simple reverse filter
/// </summary>
/// <remarks>
/// Creates a reverse filter ( Like not filter ), use not operator
/// </remarks>
public class ReverseFilter<T>(IFilter<T> filter1)
    : Filter<T>((x) => !filter1.TheyShellPass(x))
{
}

/// <summary>
/// Used when two filters are going to join other.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class JoinedFilter<T>(params IFilter<T>[] filters)
    : IJoinedFilter<T>
{
    private Dictionary<string, object>? _extraData;

    /// <inheritdoc/>
    public IFilter<T>[] Filters { get; } = filters;

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object>? ExtraData => _extraData;

    /// <summary>
    /// Check if the filters are passed here.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected abstract bool InnerTheyShellPass(T input);

    /// <inheritdoc/>
    public bool TheyShellPass(T input)
    {
        var shellPass = InnerTheyShellPass(input);
        _extraData = Filters.Where(x => x.ExtraData is not null)
            .SelectMany(x => x.ExtraData!) // extra data not null here.
#if NET8_0_OR_GREATER
            .DistinctBy(x => x.Key) // Is it required ?
#else
            .GroupBy(x => x.Key, StringComparer.Ordinal).Select(x => x.First())
#endif
            .ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
        return shellPass;
    }
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
    protected override bool InnerTheyShellPass(T input)
    {
        return Filters[0].TheyShellPass(input) &&
            Filters[1].TheyShellPass(input);
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
    protected override bool InnerTheyShellPass(T input)
    {
        return Filters[0].TheyShellPass(input) ||
            Filters[1].TheyShellPass(input);
    }
}

/// <summary>
/// Inputs for filters inside <see cref="IUpdater"/>
/// </summary>
public class UpdaterFilterInputs<T>(
    IUpdater updater,
    T input,
    Guid scopeId,
    int layerId,
    int group,
    int index)
{
    /// <summary>
    /// The updater.
    /// </summary>
    public IUpdater Updater { get; } = updater;

    /// <summary>
    /// The actual input.
    /// </summary>
    public T Input { get; } = input;

    /// <summary>
    /// The unique <see cref="Guid"/> of the scope of handling.
    /// </summary>
    /// <remarks>
    /// This scope id is same for handlers being triggered by the same update in handling chain.
    /// </remarks>
    public Guid ScopeId { get; } = scopeId;

    /// <summary>
    /// The <see cref="HandlingOptions.LayerId"/> of current handler.
    /// </summary>
    public int LayerId { get; } = layerId;

    /// <summary>
    /// The <see cref="HandlingOptions.Group"/> of this handler.
    /// </summary>
    public int Group { get; } = group;

    /// <summary>
    /// Index of the handler in it's layer.
    /// </summary>
    public int Index { get; } = index;

    internal UpdaterFilterInputs<Q> Rebase<Q>(Q newBase)
        => new(Updater, newBase, ScopeId, LayerId, Group, Index);
}

/// <summary>
/// A filter to use inside <see cref="IUpdater"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class UpdaterFilter<T> : Filter<UpdaterFilterInputs<T>>
{
    /// <summary>
    /// Creates a new instance of <see cref="UpdaterFilter{T}"/>.
    /// </summary>
    public UpdaterFilter() : base()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="UpdaterFilter{T}"/>.
    /// </summary>
    public UpdaterFilter(Func<UpdaterFilterInputs<T>, bool> filter) : base(filter)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="UpdaterFilter{T}"/>.
    /// </summary>
    public UpdaterFilter(Func<T, bool> filter)
        : base((input) => filter(input.Input))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="UpdaterFilter{T}"/>.
    /// </summary>
    public UpdaterFilter(Func<IUpdater, T, bool> filter)
        : base((input) => filter(input.Updater, input.Input))
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

