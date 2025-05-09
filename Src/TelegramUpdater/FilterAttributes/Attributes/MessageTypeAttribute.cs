namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter message by their <see cref="Message.Type"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="MessageTypeAttribute"/>.
/// </remarks>
/// <param name="messageType">Message type to filter.</param>
public sealed class MessageTypeAttribute(MessageType messageType)
    : FilterAttributeBuilder(builder => builder
        .AddFilterForUpdate<Message>(
            new((_, message) => message.Type == messageType)))
{
}
