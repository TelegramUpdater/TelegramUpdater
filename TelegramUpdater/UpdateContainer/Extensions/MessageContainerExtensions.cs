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

    /// <inheritdoc cref="TelegramBotClientExtensions.DeleteMessageAsync(ITelegramBotClient, ChatId, int, CancellationToken)"/>
    public static async Task Delete(this IContainer<Message> simpleContext)
        => await simpleContext.BotClient.DeleteMessageAsync(
            simpleContext.Update.Chat.Id, simpleContext.Update.MessageId);

    /// <summary>
    /// Updates a <see cref="Message"/> of your own with removing it and sending a new message.
    /// </summary>
    public static async Task<IContainer<Message>> ForceUpdateAsync(
        this IContainer<Message> simpleContext,
        string text,
        bool sendAsReply = true,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        bool? disableNotification = default,
        IReplyMarkup? replyMarkup = default)
    {
        if (simpleContext.Update.From?.Id != simpleContext.BotClient.BotId)
            throw new InvalidOperationException("The message should be for the bot it self.");

        await simpleContext.Delete();
        return await simpleContext.BotClient.SendTextMessageAsync(
            simpleContext.Update.Chat.Id,
            text, parseMode, messageEntities,
            disableWebpagePreview, disableNotification,
            replyToMessageId: sendAsReply ? simpleContext.Update.MessageId : 0,
            allowSendingWithoutReply: true,
            replyMarkup: replyMarkup)
            .WrapMessageAsync(simpleContext.Updater);
    }

    /// <inheritdoc cref="TelegramBotClientExtensions.SendTextMessageAsync(
    /// ITelegramBotClient, ChatId, string, ParseMode?,
    /// IEnumerable{MessageEntity}?, bool?, bool?, int?,
    /// bool?, IReplyMarkup?, CancellationToken)"/>
    public static async Task<IContainer<Message>> ResponseAsync(
        this IContainer<Message> simpleContext,
        string text,
        bool sendAsReply = true,
        ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        bool? disableNotification = default,
        IReplyMarkup? replyMarkup = default)
        => await simpleContext.BotClient.SendTextMessageAsync(
            simpleContext.Update.Chat.Id,
            text, parseMode, messageEntities,
            disableWebpagePreview, disableNotification,
            replyToMessageId: sendAsReply ? simpleContext.Update.MessageId : 0,
            allowSendingWithoutReply: true,
            replyMarkup: replyMarkup)
        .WrapMessageAsync(simpleContext.Updater);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageTextAsync(
    /// ITelegramBotClient, ChatId, int, string, ParseMode?,
    /// IEnumerable{MessageEntity}?, bool?, InlineKeyboardMarkup?,
    /// CancellationToken)"/>
    public static async Task<IContainer<Message>?> EditAsync(
        this IContainer<Message> simpleContext,
        string text, ParseMode? parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        CancellationToken cancellationToken = default)
    {
        return await simpleContext.BotClient.EditMessageTextAsync(
            simpleContext.Update.Chat.Id,
            simpleContext.Update.MessageId,
            text, parseMode, messageEntities,
            disableWebpagePreview, inlineKeyboardMarkup,
            cancellationToken)
            .WrapMessageAsync(simpleContext.Updater);
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
