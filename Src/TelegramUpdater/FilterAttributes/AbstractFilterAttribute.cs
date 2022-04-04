namespace TelegramUpdater.FilterAttributes
{
    /// <summary>
    /// Abstract base for filter attributes.
    /// </summary>
    [
        AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = true)
    ]
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

        /// <summary>
        /// Indicates how the filter is going to be appended to others.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <see langword="true"/>, to append as an <b>Or Filter</b>.
        /// </item>
        /// <item>
        /// <see langword="false"/>, to append as an <b>And Filter</b>
        /// </item>
        /// </list>
        /// Defaults to <see langword="false"/> to append as an <b>And Filter</b>.
        /// </remarks>
        public bool ApplyAsOr { get; set; } = false;

        /// <summary>
        /// Indicates if the filter should be reversed.
        /// </summary>
        /// <remarks>
        /// Defaults to <see langword="false"/>.
        /// </remarks>
        public bool Reverse { get; set; } = false;

        /// <summary>
        /// Indicates if this filter is the start of a new batch of filters
        /// </summary>
        public bool BatchStart { get; set; } = false;

        /// <summary>
        /// Indicates if this batch should append as OR.
        /// </summary>
        public bool ReverseBatch { get; set; } = false;

        /// <summary>
        /// Indicates if this batch should be reversed.
        /// </summary>
        public bool OrBatch { get; set; } = false;
    }
}
