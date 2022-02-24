namespace TelegramUpdater.FilterAttributes.Attributes
{
    /// <summary>
    /// Filter private messages.
    /// </summary>
    public sealed class PrivateAttribute : FilterAttributeBuilder
    {
        /// <summary>
        /// Create an instance of private filter attribute.
        /// </summary>
        public PrivateAttribute()
            : base(x => x.AddFilterForUpdate(FilterCutify.PM()))
        {
        }
    }
}
