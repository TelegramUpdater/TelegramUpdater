// Ignore Spelling: Webpage inline

using Telegram.Bot.Types.ReplyMarkups;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
    public static async Task Delete(this IBaseContainer<Message> simpleContext, CancellationToken cancellationToken = default)
        => await simpleContext.BotClient.DeleteMessage(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Updates a <see cref="Message"/> of your own with removing it and sending a new message.
    /// </summary>
    public static async Task<IBaseContainer<Message>> ForceUpdate(
        this IBaseContainer<Message> simpleContext,
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
    public static async Task<IBaseContainer<Message>> Response(
        this IBaseContainer<Message> simpleContext,
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
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<Message> simpleContext,
        string text,
        ParseMode parseMode = default,
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
            linkPreviewOptions: disableWebpagePreview,
            replyMarkup: replyMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken)
            .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageLiveLocation(ITelegramBotClient, ChatId, int, double, double, int?, double?, int?, int?, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<Message> simpleContext,
        double latitude,
        double longitude,
        int? livePeriod = default,
        float? horizontalAccuracy = default,
        int? heading = default,
        int? proximityAlertRadius = default,
        InlineKeyboardMarkup? replyMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        return await simpleContext.BotClient.EditMessageLiveLocation(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            latitude: latitude,
            longitude: longitude,
            livePeriod: livePeriod,
            horizontalAccuracy: horizontalAccuracy,
            heading: heading,
            proximityAlertRadius: proximityAlertRadius,
            replyMarkup: replyMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageMedia(ITelegramBotClient, ChatId, int, InputMedia, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<Message> simpleContext,
        InputMedia inputMedia,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        return await simpleContext.BotClient.EditMessageMedia(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            media: inputMedia,
            replyMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageCaption(ITelegramBotClient, string, string?, ParseMode, IEnumerable{MessageEntity}?, bool, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> EditCaption(
        this IBaseContainer<Message> simpleContext,
        string caption,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? captionEntities = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        return await simpleContext.BotClient.EditMessageCaption(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            caption: caption,
            parseMode: parseMode,
            captionEntities: captionEntities,
            replyMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageReplyMarkup(ITelegramBotClient, string, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<Message> simpleContext,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        return await simpleContext.BotClient.EditMessageReplyMarkup(
            chatId: simpleContext.Update.Chat.Id,
            messageId: simpleContext.Update.MessageId,
            replyMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ChatType.Private"/>
    public static bool IsPrivate(this IBaseContainer<Message> simpleContext)
        => simpleContext.Update.Chat.Type == ChatType.Private;

    /// <summary>
    /// Message is sent to group chat.
    /// </summary>
    public static bool IsGroup(this IBaseContainer<Message> simpleContext)
        => simpleContext.Update.Chat.Type == ChatType.Supergroup ||
            simpleContext.Update.Chat.Type == ChatType.Group;
}
