// Ignore Spelling: inline Webpage

using Telegram.Bot.Types.ReplyMarkups;

// NS not matching the folder structure is intended.
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of extension methods for <see cref="CallbackQuery"/> containers.
/// </summary>
public static class CallbackQueryContainerExtensions
{
    /// <inheritdoc cref="User.Id"/>
    public static long SenderId(this IContainer<CallbackQuery> simpleContext)
        => simpleContext.Update.From.Id;

    /// <inheritdoc cref="Message.From"/>
    public static User Sender(this IContainer<CallbackQuery> simpleContext)
        => simpleContext.Update.From;

    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)" />
    public static async Task<IBaseContainer<Message>> SendChatMessage(
        this IBaseContainer<CallbackQuery> simpleContext,
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
    {
        if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.SendMessage(
                chatId: simpleContext.Update.Message.Chat.Id,
                text: text,
                parseMode: parseMode,
                replyParameters: new ReplyParameters()
                {
                    AllowSendingWithoutReply = allowSendingWithoutReply,
                    MessageId = sendAsReply ? simpleContext.Update.Message.MessageId : 0,
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
        }

        throw new InvalidOperationException("Can't send message for inline message calls.");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)" />
    public static async Task<IBaseContainer<Message>> SendMessage(
        this IBaseContainer<CallbackQuery> simpleContext,
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
    {
        return await simpleContext(
            chatId: simpleContext.Update.From.Id,
            text: text,
            parseMode: parseMode,
            replyParameters: new ReplyParameters()
            {
                AllowSendingWithoutReply = allowSendingWithoutReply,
                MessageId = sendAsReply ? (simpleContext.Update.Message?.MessageId?? 0) : 0,
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
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.AnswerCallbackQuery(ITelegramBotClient, string, string?, bool, string?, int?, CancellationToken)"/>
    public static async Task Answer(
        this IContainer<CallbackQuery> simpleContext,
        string? text = default, bool showAlert = default,
        string? url = default, int? cacheTime = default,
        CancellationToken cancellationToken = default)
        => await simpleContext.BotClient.AnswerCallbackQuery(
            callbackQueryId: simpleContext.Update.Id,
            text: text,
            showAlert: showAlert,
            url: url,
            cacheTime: cacheTime,
            cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageText(ITelegramBotClient, ChatId, int, string, ParseMode, IEnumerable{MessageEntity}?, LinkPreviewOptions?, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<CallbackQuery> simpleContext,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageText(
                inlineMessageId: simpleContext.Update.InlineMessageId,
                text: text,
                parseMode: parseMode,
                entities: messageEntities,
                linkPreviewOptions: disableWebpagePreview,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return null;
        }

        if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageText(
                chatId: simpleContext.Update.Message.Chat.Id,
                messageId: simpleContext.Update.Message.MessageId,
                text: text,
                parseMode: parseMode,
                entities: messageEntities,
                linkPreviewOptions: disableWebpagePreview,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken)
            .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageLiveLocation(ITelegramBotClient, ChatId, int, double, double, int?, double?, int?, int?, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<CallbackQuery> simpleContext,
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
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageLiveLocation(
                inlineMessageId: simpleContext.Update.InlineMessageId,
                latitude: latitude,
                longitude: longitude,
                livePeriod: livePeriod,
                horizontalAccuracy: horizontalAccuracy,
                heading: heading,
                proximityAlertRadius: proximityAlertRadius,
                replyMarkup: replyMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return null;
        }

        if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageLiveLocation(
                chatId: simpleContext.Update.Message.Chat.Id,
                messageId: simpleContext.Update.Message.MessageId,
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

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageMedia(ITelegramBotClient, ChatId, int, InputMedia, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<CallbackQuery> simpleContext,
        InputMedia inputMedia,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageMedia(
                inlineMessageId: simpleContext.Update.InlineMessageId,
                media: inputMedia,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return null;
        }

        if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageMedia(
                chatId: simpleContext.Update.Message.Chat.Id,
                messageId: simpleContext.Update.Message.MessageId,
                media: inputMedia,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken)
            .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageCaption(ITelegramBotClient, string, string?, ParseMode, IEnumerable{MessageEntity}?, bool, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> EditCaption(
        this IBaseContainer<CallbackQuery> simpleContext,
        string caption,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? captionEntities = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageCaption(
                inlineMessageId: simpleContext.Update.InlineMessageId,
                caption: caption,
                parseMode: parseMode,
                captionEntities: captionEntities,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return null;
        }

        if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageCaption(
                chatId: simpleContext.Update.Message.Chat.Id,
                messageId: simpleContext.Update.Message.MessageId,
                caption: caption,
                parseMode: parseMode,
                captionEntities: captionEntities,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken)
            .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageReplyMarkup(ITelegramBotClient, string, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public static async Task<IBaseContainer<Message>?> Edit(
        this IBaseContainer<CallbackQuery> simpleContext,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageReplyMarkup(
                inlineMessageId: simpleContext.Update.InlineMessageId,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return null;
        }

        if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageReplyMarkup(
                chatId: simpleContext.Update.Message.Chat.Id,
                messageId: simpleContext.Update.Message.MessageId,
                replyMarkup: inlineKeyboardMarkup,
                businessConnectionId: businessConnectionId,
                cancellationToken: cancellationToken)
            .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }
}
