namespace Telegram.Bot.Types;

/// <summary>
/// Extensions for <see cref="Update"/>
/// </summary>
public static class UpdateExtensions
{
    /// <summary>
    /// Gets sender id of an update. ( usually .From or .SenderChat )
    /// </summary>
    public static long? GetSenderId(this Update update)
    {
        return update switch
        {
            { Message.From: { } from } => from.Id,
            { Message.SenderChat: { } chat } => chat.Id,
            { EditedMessage.From: { } from } => from.Id,
            { EditedMessage.SenderChat: { } chat } => chat.Id,
            { ChannelPost.From: { } from } => from.Id,
            { ChannelPost.SenderChat: { } chat } => chat.Id,
            { EditedChannelPost.From: { } from } => from.Id,
            { EditedChannelPost.SenderChat: { } chat } => chat.Id,
            { CallbackQuery: { } call } => call.From.Id,
            { InlineQuery.From: { } from } => from.Id,
            { PollAnswer.User: { } user } => user.Id,
            { PreCheckoutQuery.From: { } from } => from.Id,
            { ShippingQuery.From: { } from } => from.Id,
            { ChosenInlineResult.From: { } from } => from.Id,
            { ChatJoinRequest.From: { } from } => from.Id,
            { ChatMember.From: { } from } => from.Id,
            { MyChatMember.From: { } from } => from.Id,
            _ => null,
        };
    }

    /// <summary>
    /// Resolve a chat id from <see cref="Update"/>.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static long? GetChatId(this Update update)
    {
        return update switch
        {
            { Message.Chat: { } chat } => chat.Id,
            { Message.SenderChat: { } chat } => chat.Id,
            { EditedMessage.Chat: { } chat } => chat.Id,
            { EditedMessage.SenderChat: { } chat } => chat.Id,
            { ChannelPost.Chat: { } chat } => chat.Id,
            { ChannelPost.SenderChat: { } chat } => chat.Id,
            { EditedChannelPost.Chat: { } chat } => chat.Id,
            { EditedChannelPost.SenderChat: { } chat } => chat.Id,
            { CallbackQuery.Message.Chat: { } chat } => chat.Id,
            { ChatJoinRequest.Chat: { } chat } => chat.Id,
            { ChatMember.Chat: { } chat } => chat.Id,
            { MyChatMember.Chat: { } chat } => chat.Id,
            _ => null,
        };
    }
}
