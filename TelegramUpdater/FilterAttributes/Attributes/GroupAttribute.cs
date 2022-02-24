namespace TelegramUpdater.FilterAttributes.Attributes
{
    /// <summary>
    /// Filter attribute for <see cref="FilterCutify.Group"/>
    /// </summary>
    public sealed class GroupAttribute : FilterAttributeBuilder
    {
        /// <summary>
        /// Init <see cref="GroupAttribute"/>.
        /// </summary>
        public GroupAttribute()
            : base(x => x.AddFilterForUpdate(FilterCutify.Group()))
        {
        }
    }
}
