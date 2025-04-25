using TelegramUpdater.Helpers;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Create filter attribute for <see cref="FilterCutify.InChatType(ChatTypeFlags)"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="ChatTypeAttribute"/>.
/// </remarks>
/// <param name="flags">Chat type flags.</param>
public class ChatTypeAttribute(ChatTypeFlags flags)
    : FilterAttributeBuilder(x => x.AddFilterForUpdate(FilterCutify.InChatType(flags)))
{
}
