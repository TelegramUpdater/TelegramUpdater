using System;

namespace TelegramUpdater
{
    /// <summary>
    /// A simple basic filter
    /// </summary>
    /// <typeparam name="T">Object type that filter is gonna apply to</typeparam>
    public class Filter<T>
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
}
