using TelegramUpdater.Helpers;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Create filter attribute for <see cref="FilterCutify.InChatType(ChatTypeFlags)"/>.
/// </summary>
public class ChatTypeAttribute : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="ChatTypeAttribute"/>.
    /// </summary>
    /// <param name="flags">Chat type flags.</param>
    public ChatTypeAttribute(ChatTypeFlags flags)
        : base(x => x.AddFilterForUpdate(FilterCutify.InChatType(flags)))
    { }
}
