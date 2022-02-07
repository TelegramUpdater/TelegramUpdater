using System;

namespace TelegramUpdater
{
    /// <summary>
    /// Use this attribute to apply a filter on <see cref="UpdateHandlers.ScopedHandlers.IScopedUpdateHandler"/>
    /// </summary>
    /// <remarks>
    /// The filter should have a parameterless constructor.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class ApplyFilterAttribute : Attribute
    {
        public Type FilterType { get; }

        /// <summary>
        /// Use this attribute to apply a filter on <see cref="UpdateHandlers.ScopedHandlers.IScopedUpdateHandler"/>
        /// </summary>
        /// <remarks>
        /// The filter should have a parameterless constructor.
        /// </remarks>
        /// <param name="filterType">Type of your filter which is a child class of <see cref="Filter{T}"/></param>
        public ApplyFilterAttribute(Type filterType)
        {
            FilterType = filterType;
        }
    }
}
