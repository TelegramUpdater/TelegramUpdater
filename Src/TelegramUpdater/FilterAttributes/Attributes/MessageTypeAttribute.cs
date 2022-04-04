namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter message by their <see cref="Message.Type"/>.
/// </summary>
public sealed class MessageTypeAttribute : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="MessageTypeAttribute"/>.
    /// </summary>
    /// <param name="messageType">Message type to filter.</param>
    public MessageTypeAttribute(MessageType messageType)
        : base(builder => builder.AddFilterForUpdate<Message>(
            new((_, message) => message.Type == messageType)))
    {
    }
}
