using TelegramUpdater.UpdateContainer.Tags;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// A container for <see cref="Update.CallbackQuery"/> only.
/// </summary>
public sealed class CallbackQueryContainer : AbstractUpdateContainer<CallbackQuery>,
    IChatExtractable,
    IChatIdExtractable,
    ISenderIdExtractable,
    ISenderUserExtractable
{
    internal CallbackQueryContainer(
        HandlerInput input,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(x => x.CallbackQuery, input, extraObjects)
    {
    }

    /// <inheritdoc/>
    public Chat? GetChat() => Update.Message?.Chat;

    /// <inheritdoc/>
    public long? GetChatId() => Update.Message?.Chat?.Id;

    /// <inheritdoc/>
    public long? GetSenderId() => Update.From?.Id;

    /// <inheritdoc/>
    public User? GetSenderUser() => Update.From;
}
