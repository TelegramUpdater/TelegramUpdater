namespace TelegramUpdater
{
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
    }

    /// <summary>
    /// A simple basic filter
    /// </summary>
    /// <typeparam name="T">Object type that filter is gonna apply to</typeparam>
    public class Filter<T> : IFilter<T>
    {
        private readonly Func<T, bool> _filter;

        /// <summary>
        /// Creates a simple basic filter
        /// </summary>
        /// <param name="filter">A function to check the input and return a boolean</param>
        public Filter(Func<T, bool> filter) => _filter = filter;

        /// <summary>
        /// Indicates if an input of type <typeparamref name="T"/> can pass this filter
        /// </summary>
        /// <param name="input">The input value to check</param>
        /// <returns></returns>
        public virtual bool TheyShellPass(T input)
            => input != null && _filter(input);

        /// <summary>
        /// Converts a filter to a fucntion.
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
        public Filter<T> And(Filter<T> simpleFilter)
            => new AndFilter<T>(this, simpleFilter);

        /// <summary>
        /// Returns an <see cref="OrFilter{T}"/> version.
        /// <para>
        /// This Or <paramref name="simpleFilter"/>
        /// </para>
        /// </summary>
        public Filter<T> Or(Filter<T> simpleFilter)
            => new OrFilter<T>(this, simpleFilter);

        /// <summary>
        /// Returns a reversed version of this <see cref="Filter{T}"/>
        /// </summary>
        /// <returns></returns>
        public Filter<T> Reverse() => new ReverseFilter<T>(this);

        /// <summary>
        /// Converts a <paramref name="filter"/> to <see cref="Filter{T}"/>
        /// </summary>
        /// <param name="filter"></param>

        public static implicit operator Filter<T>(Func<T, bool> filter) => new Filter<T>(filter);

        /// <summary>
        /// Creates an <see cref="AndFilter{T}"/>
        /// </summary>
        public static Filter<T> operator &(Filter<T> a, Filter<T> b)
            => new AndFilter<T>(a, b);

        /// <summary>
        /// Creates an <see cref="OrFilter{T}"/>
        /// </summary>
        public static Filter<T> operator |(Filter<T> a, Filter<T> b)
            => new OrFilter<T>(a, b);

        /// <summary>
        /// Creates a reversed version of this <see cref="Filter{T}"/>
        /// </summary>
        public static Filter<T> operator ~(Filter<T> a)
            => new ReverseFilter<T>(a);
    }

    /// <summary>
    /// Creates a simple and filter
    /// </summary>
    public class AndFilter<T> : Filter<T>
    {
        /// <summary>
        /// Creates a and filter ( Like filter1 and filter2 ), use and operator
        /// </summary>
        public AndFilter(Filter<T> filter1, Filter<T> filter2)
            : base(x => filter1.TheyShellPass(x) && filter2.TheyShellPass(x))
        { }
    }

    /// <summary>
    /// Creates a simple or filter
    /// </summary>
    public class OrFilter<T> : Filter<T>
    {
        /// <summary>
        /// Creates an or filter ( Like filter1 or filter2 ), use or operator
        /// </summary>
        public OrFilter(Filter<T> filter1, Filter<T> filter2)
            : base(x => filter1.TheyShellPass(x) || filter2.TheyShellPass(x))
        { }
    }

    /// <summary>
    /// Creates a simple reverse filter
    /// </summary>
    public class ReverseFilter<T> : Filter<T>
    {
        /// <summary>
        /// Creates a reverse filter ( Like not filter ), use not operator
        /// </summary>
        public ReverseFilter(Filter<T> filter1)
            : base(x => !filter1.TheyShellPass(x))
        { }
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
    }
}
