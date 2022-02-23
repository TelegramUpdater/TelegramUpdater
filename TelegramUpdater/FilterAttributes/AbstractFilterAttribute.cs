using System;

namespace TelegramUpdater.FilterAttributes
{
    /// <summary>
    /// Abstract base for filter attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class AbstractFilterAttribute : Attribute
    {
        /// <summary>
        /// Get the inner filter of this attribute for possible type of <typeparamref name="TUpdate"/>.
        /// </summary>
        /// <typeparam name="TUpdate">Your update type.</typeparam>
        internal Filter<TUpdate> GetFilterTypeOf<TUpdate>() where TUpdate : class
        {
            return (Filter<TUpdate>)GetFilterTypeOf(typeof(TUpdate));
        }

        /// <summary>
        /// Get the inner filter for type <paramref name="requestedType"/> if possible.
        /// </summary>
        protected internal abstract object GetFilterTypeOf(Type requestedType);
    }
}
