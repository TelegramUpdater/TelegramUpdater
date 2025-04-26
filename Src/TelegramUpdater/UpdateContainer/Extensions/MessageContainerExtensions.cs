// Ignore Spelling: Webpage

using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// A set of extension methods for message <see cref="IContainer{T}"/>.
/// </summary>
public static class MessageContainerExtensions
{
    /// <inheritdoc cref="User.Id"/>
    public static long? SenderId(this IContainer<Message> simpleContext)
        => simpleContext.Update.From?.Id;

    /// <inheritdoc cref="Message.From"/>
    public static User? Sender(this IContainer<Message> simpleContext)
        => simpleContext.Update.From;

    /// <inheritdoc cref="TelegramBotClientExtensions.DeleteMessage(ITelegramBotClient, ChatId, int, CancellationToken)"/>
    public static async Task Delete(this IContainer<Message> simpleContext, CancellationToken cancellationToken = default)
        => await simpleContext.BotClient.DeleteMessage(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Updates a <see cref="Message"/> of your own with removing it and sending a new message.
    /// </summary>
    public static async Task<IContainer<Message>> ForceUpdateAsync(
        this IContainer<Message> simpleContext,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        int? messageThreadId = default,
        bool disableNotification = default,
        ReplyMarkup? replyMarkup = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.From?.Id != simpleContext.BotClient.BotId)
            throw new InvalidOperationException("The message should be for the bot it self.");

        await simpleContext.Delete(cancellationToken: cancellationToken).ConfigureAwait(false);
        return await simpleContext.BotClient.SendMessage(
            chatId: simpleContext.Update.Chat.Id,
            text: text,
            parseMode: parseMode,
            replyParameters: new ReplyParameters(),
            replyMarkup: replyMarkup,
            linkPreviewOptions: disableWebpagePreview,
            messageThreadId: messageThreadId,
            entities: messageEntities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)"/>
    public static async Task<IContainer<Message>> ResponseAsync(
        this IContainer<Message> simpleContext,
        string text,
        bool sendAsReply = true,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        int? messageThreadId = default,
        bool disableNotification = default,
        ReplyMarkup? replyMarkup = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        bool allowSendingWithoutReply = true,
        CancellationToken cancellationToken = default)
        => await simpleContext.BotClient.SendMessage(
            chatId: simpleContext.Update.Chat.Id,
            text: text,
            parseMode: parseMode,
            replyParameters: new ReplyParameters()
            {
                AllowSendingWithoutReply = allowSendingWithoutReply,
                MessageId = sendAsReply ? simpleContext.Update.MessageId : 0,
            },
            replyMarkup: replyMarkup,
            linkPreviewOptions: disableWebpagePreview,
            messageThreadId: messageThreadId,
            entities: messageEntities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageText(ITelegramBotClient, ChatId, int, string, ParseMode, IEnumerable{MessageEntity}?, LinkPreviewOptions?, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<Message> simpleContext,
        string text, ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        InlineKeyboardMarkup? replyMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        return await simpleContext.BotClient.EditMessageText(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            text: text,
            parseMode: parseMode,
            entities: messageEntities,
            linkPreviewOptions:disableWebpagePreview,
            replyMarkup: replyMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken)
            .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ChatType.Private"/>
    public static bool IsPrivate(this IContainer<Message> simpleContext)
        => simpleContext.Update.Chat.Type == ChatType.Private;

    /// <summary>
    /// Message is sent to group chat.
    /// </summary>
    public static bool IsGroup(this IContainer<Message> simpleContext)
        => simpleContext.Update.Chat.Type == ChatType.Supergroup ||
            simpleContext.Update.Chat.Type == ChatType.Group;
}
