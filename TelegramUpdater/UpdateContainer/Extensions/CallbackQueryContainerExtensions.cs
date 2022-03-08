using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramUpdater.UpdateContainer;

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

    /// <inheritdoc cref="TelegramBotClientExtensions.SendTextMessageAsync(
    /// ITelegramBotClient, ChatId, string, ParseMode?,
    /// IEnumerable{MessageEntity}?, bool?, bool?, int?,
    /// bool?, IReplyMarkup?, CancellationToken)"/>
    public static async Task<IContainer<Message>> SendAsync(
        this IContainer<CallbackQuery> simpleContext,
        string text,
        bool sendAsReply = true,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        bool? disableNotification = default,
        IReplyMarkup? replyMarkup = default)
    {
        if (simpleContext.Update.Message != null)
        {

            return await simpleContext.BotClient.SendTextMessageAsync(
                simpleContext.Update.Message.Chat.Id,
                text, parseMode, messageEntities,
                disableWebpagePreview, disableNotification,
                replyToMessageId: sendAsReply ? simpleContext.Update.Message.MessageId : 0,
                allowSendingWithoutReply: true,
                replyMarkup: replyMarkup)
                .WrapMessageAsync(simpleContext.Updater);
        }
        else
        {
            throw new InvalidOperationException("Can't send message for inline message calls.");
        }
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.AnswerCallbackQueryAsync(
    /// ITelegramBotClient, string, string?, bool?, string?, int?, CancellationToken)"/>
    public static async Task AnswerAsync(
        this IContainer<CallbackQuery> simpleContext,
        string? text = default, bool? showAlert = default,
        string? url = default, int? cacheTime = default,
        CancellationToken cancellationToken = default)
        => await simpleContext.BotClient.AnswerCallbackQueryAsync(
            simpleContext.Update.Id, text, showAlert, url,
            cacheTime, cancellationToken);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageTextAsync(
    /// ITelegramBotClient, ChatId, int, string, ParseMode?,
    /// IEnumerable{MessageEntity}?, bool?, InlineKeyboardMarkup?,
    /// CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<CallbackQuery> simpleContext,
        string text,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageTextAsync(simpleContext.Update.InlineMessageId,
                                                            text,
                                                            parseMode,
                                                            messageEntities,
                                                            disableWebpagePreview,
                                                            inlineKeyboardMarkup,
                                                            cancellationToken);
            return null;
        }
        else if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageTextAsync(simpleContext.Update.Message.Chat.Id,
                                                            simpleContext.Update.Message.MessageId,
                                                            text,
                                                            parseMode,
                                                            messageEntities,
                                                            disableWebpagePreview,
                                                            inlineKeyboardMarkup,
                                                            cancellationToken)
                .WrapMessageAsync(simpleContext.Updater);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageLiveLocationAsync(
    /// ITelegramBotClient, ChatId, int, double,
    /// double, float?, int?, int?, InlineKeyboardMarkup?,
    /// CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<CallbackQuery> simpleContext,
        double latitude,
        double longitude,
        float? horizontalAccuracy = default,
        int? heading = default,
        int? proximityAlertRadius = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageLiveLocationAsync(
                simpleContext.Update.InlineMessageId,
                latitude, longitude, horizontalAccuracy,
                heading, proximityAlertRadius,
                inlineKeyboardMarkup,
                cancellationToken);
            return null;
        }
        else if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageLiveLocationAsync(
                simpleContext.Update.Message.Chat.Id,
                simpleContext.Update.Message.MessageId,
                latitude, longitude, horizontalAccuracy,
                heading, proximityAlertRadius,
                inlineKeyboardMarkup, cancellationToken)
                .WrapMessageAsync(simpleContext.Updater);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageMediaAsync(
    /// ITelegramBotClient, ChatId, int, InputMediaBase,
    /// InlineKeyboardMarkup?, CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<CallbackQuery> simpleContext,
        InputMediaBase inputMediaBase,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageMediaAsync(
                simpleContext.Update.InlineMessageId,
                inputMediaBase,
                inlineKeyboardMarkup,
                cancellationToken);
            return null;
        }
        else if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageMediaAsync(
                simpleContext.Update.Message.Chat.Id,
                simpleContext.Update.Message.MessageId,
                inputMediaBase,
                inlineKeyboardMarkup,
                cancellationToken)
                .WrapMessageAsync(simpleContext.Updater);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageCaptionAsync(
    /// ITelegramBotClient, ChatId, int, string?, ParseMode?,
    /// IEnumerable{MessageEntity}?, InlineKeyboardMarkup?, CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<CallbackQuery> simpleContext,
        string caption,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageCaptionAsync(
                simpleContext.Update.InlineMessageId,
                caption, parseMode, messageEntities,
                inlineKeyboardMarkup, cancellationToken);
            return null;
        }
        else if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageCaptionAsync(
                simpleContext.Update.Message.Chat.Id,
                simpleContext.Update.Message.MessageId,
                caption, parseMode, messageEntities,
                inlineKeyboardMarkup, cancellationToken)
                .WrapMessageAsync(simpleContext.Updater);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageReplyMarkupAsync(
    /// ITelegramBotClient, ChatId, int, InlineKeyboardMarkup?, CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<CallbackQuery> simpleContext,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        CancellationToken cancellationToken = default)
    {
        if (simpleContext.Update.InlineMessageId != null)
        {
            await simpleContext.BotClient.EditMessageReplyMarkupAsync(
                simpleContext.Update.InlineMessageId,
                inlineKeyboardMarkup,
                cancellationToken);
            return null;
        }
        else if (simpleContext.Update.Message != null)
        {
            return await simpleContext.BotClient.EditMessageReplyMarkupAsync(
                simpleContext.Update.Message.Chat.Id,
                simpleContext.Update.Message.MessageId,
                inlineKeyboardMarkup,
                cancellationToken)
                .WrapMessageAsync(simpleContext.Updater);
        }

        throw new InvalidOperationException("InlineMessageId and Message are both null!");
    }
}
