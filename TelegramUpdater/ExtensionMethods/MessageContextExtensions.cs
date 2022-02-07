using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater
{
    public static class MessageContextExtensions
    {
        /// <summary>
        /// Deletes a message
        /// </summary>
        /// <param name="simpleContext"></param>
        /// <returns></returns>
        public static async Task Delete(this UpdateContainerAbs<Message> simpleContext)
            => await simpleContext.BotClient.DeleteMessageAsync(
                simpleContext.Update.Chat.Id, simpleContext.Update.MessageId);

        /// <summary>
        /// Updates a <see cref="Message"/> of your own with removing it and sending a new message.
        /// </summary>
        public static async Task<UpdateContainerAbs<Message>> ForceUpdate(this UpdateContainerAbs<Message> simpleContext,
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
            return await simpleContext.BotClient.SendTextMessageAsync(simpleContext.Update.Chat.Id,
                                                                   text,
                                                                   parseMode,
                                                                   messageEntities,
                                                                   disableWebpagePreview,
                                                                   disableNotification,
                                                                   replyToMessageId: sendAsReply ? simpleContext.Update.MessageId : 0,
                                                                   allowSendingWithoutReply: true,
                                                                   replyMarkup: replyMarkup)
                    .WrapAsync(simpleContext);
        }

        /// <summary>
        /// Quickest possible way to response to a message
        /// Shortcut for <c>SendTextMessageAsync</c>
        /// </summary>
        /// <param name="text">Text to response</param>
        /// <param name="sendAsReply">To send it as a replied message if possible.</param>
        /// <returns></returns>
        public static async Task<UpdateContainerAbs<Message>> Response(this UpdateContainerAbs<Message> simpleContext,
                                                                  string text,
                                                                  bool sendAsReply = true,
                                                                  ParseMode? parseMode = default,
                                                                  IEnumerable<MessageEntity>? messageEntities = default,
                                                                  bool? disableWebpagePreview = default,
                                                                  bool? disableNotification = default,
                                                                  IReplyMarkup? replyMarkup = default)
            => await simpleContext.BotClient.SendTextMessageAsync(simpleContext.Update.Chat.Id,
                                                               text,
                                                               parseMode,
                                                               messageEntities,
                                                               disableWebpagePreview,
                                                               disableNotification,
                                                               replyToMessageId: sendAsReply ? simpleContext.Update.MessageId : 0,
                                                               allowSendingWithoutReply: true,
                                                               replyMarkup: replyMarkup)
                    .WrapAsync(simpleContext);

        /// <summary>
        /// Edits a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<UpdateContainerAbs<Message>?> Edit(this UpdateContainerAbs<Message> simpleContext,
                                                string text,
                                                ParseMode? parseMode = default,
                                                IEnumerable<MessageEntity>? messageEntities = default,
                                                bool? disableWebpagePreview = default,
                                                InlineKeyboardMarkup? inlineKeyboardMarkup = default,
                                                CancellationToken cancellationToken = default)
        {
            return await simpleContext.BotClient.EditMessageTextAsync(simpleContext.Update.Chat.Id,
                                                            simpleContext.Update.MessageId,
                                                            text,
                                                            parseMode,
                                                            messageEntities,
                                                            disableWebpagePreview,
                                                            inlineKeyboardMarkup,
                                                            cancellationToken)
                .WrapAsync(simpleContext);
        }

        /// <summary>
        /// Message is sent to private chat.
        /// </summary>
        public static bool IsPrivate(this UpdateContainerAbs<Message> simpleContext)
            => simpleContext.Update.Chat.Type == ChatType.Private;

        /// <summary>
        /// Message is sent to group chat.
        /// </summary>
        public static bool IsGroup(this UpdateContainerAbs<Message> simpleContext)
            => simpleContext.Update.Chat.Type == ChatType.Supergroup ||
                simpleContext.Update.Chat.Type == ChatType.Group;
    }
}
