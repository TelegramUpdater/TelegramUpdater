namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter attibute for <see cref="FilterCutify.Text"/>
/// </summary>
public sealed class TextAttribute : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="TextAttribute"/>.
    /// </summary>
    public TextAttribute()
        : base(x => x.AddFilterForUpdate(FilterCutify.Text()))
    {
    }
}
