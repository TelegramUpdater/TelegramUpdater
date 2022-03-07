namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter the messages that are replied to another message
/// </summary>
public sealed class RepliedAttribute : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="RepliedAttribute"/>.
    /// </summary>
    public RepliedAttribute()
        : base(x => x.AddFilterForUpdate(FilterCutify.Replied()))
    {
    }
}
