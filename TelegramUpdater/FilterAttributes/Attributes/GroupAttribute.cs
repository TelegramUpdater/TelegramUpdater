namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter attribute for <see cref="FilterCutify.Group"/>
/// </summary>
public sealed class GroupAttribute : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="GroupAttribute"/>.
    /// </summary>
    public GroupAttribute()
        : base(x => x.AddFilterForUpdate(FilterCutify.Group()))
    {
    }
}
