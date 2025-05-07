namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter private messages.
/// </summary>
public sealed class PrivateAttribute : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="PrivateAttribute"/>.
    /// </summary>
    public PrivateAttribute()
        : base(x => x.AddFilterForUpdate(ReadyFilters.PM()))
    {
    }
}
