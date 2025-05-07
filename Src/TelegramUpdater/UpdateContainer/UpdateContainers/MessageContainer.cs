using TelegramUpdater.UpdateContainer.Tags;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// An update container for <see cref="Update.Message"/> only.
/// </summary>
public sealed class MessageContainer : AbstractUpdateContainer<Message>,
    IChatExtractable,
    IChatIdExtractable,
    ISenderIdExtractable,
    ISenderUserExtractable,
    ISenderChatExtractable
{
    internal MessageContainer(
        HandlerInput input,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(update => update.GetInnerUpdate<Message>(), input, extraObjects)
    {
    }

    /// <inheritdoc/>
    public Chat? GetChat() => Update.Chat;

    /// <inheritdoc/>
    public long? GetChatId() => Update.Chat.Id;

    /// <inheritdoc/>
    public Chat? GetSenderChat() => Update.SenderChat;

    /// <inheritdoc/>
    public long? GetSenderId() => Update.From?.Id ?? Update.SenderChat?.Id;

    /// <inheritdoc/>
    public User? GetSenderUser() => Update.From;
}
