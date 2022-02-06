using System;
using System.Diagnostics.CodeAnalysis;

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

        public virtual bool TheyShellPass([NotNullWhen(true)]T input)
            => input != null && _filter(input);

        public static implicit operator Func<T, bool>(Filter<T> filter)
            => filter.TheyShellPass;

        public Filter<T> And(Filter<T> simpleFilter)
            => new AndFilter<T>(this, simpleFilter);

        public Filter<T> Or(Filter<T> simpleFilter)
            => new OrFilter<T>(this, simpleFilter);

        public Filter<T> Reverse() => new ReverseFilter<T>(this);


        public static implicit operator Filter<T>(Func<T, bool> filter) => new Filter<T>(filter);

        public static Filter<T> operator &(Filter<T> a, Filter<T> b)
            => new AndFilter<T>(a, b);

        public static Filter<T> operator |(Filter<T> a, Filter<T> b)
            => new OrFilter<T>(a, b);

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
