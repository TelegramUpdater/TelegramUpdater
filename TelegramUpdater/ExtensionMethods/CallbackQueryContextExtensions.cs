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
    public static class CallbackQueryContextExtensions
    {
        public static long SenderId(this IContainer<CallbackQuery> simpleContext)
            => simpleContext.Update.From.Id;

        public static User Sender(this IContainer<CallbackQuery> simpleContext)
            => simpleContext.Update.From;

        public static async Task<Message> Send(this IContainer<CallbackQuery> simpleContext,
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

                return await simpleContext.BotClient.SendTextMessageAsync(simpleContext.Update.Message.Chat.Id,
                                                                       text,
                                                                       parseMode,
                                                                       messageEntities,
                                                                       disableWebpagePreview,
                                                                       disableNotification,
                                                                       replyToMessageId: sendAsReply ? simpleContext.Update.Message.MessageId : 0,
                                                                       allowSendingWithoutReply: true,
                                                                       replyMarkup: replyMarkup);
            }
            else
            {
                throw new InvalidOperationException("Can't send message for inline message calls.");
            }
        }

        /// <summary>
        /// Answers a <see cref="CallbackQuery"/>.
        /// Shortcut for <c>AnswerCallbackQueryAsync</c>
        /// </summary>
        public static async Task Answer(this IContainer<CallbackQuery> simpleContext, string? text = default,
                                        bool? showAlert = default, string? url = default, int? cacheTime = default,
                                        CancellationToken cancellationToken = default)
            => await simpleContext.BotClient.AnswerCallbackQueryAsync(simpleContext.Update.Id, text, showAlert, url,
                                                                   cacheTime, cancellationToken);

        /// <summary>
        /// Edits a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Message?> Edit(this IContainer<CallbackQuery> simpleContext,
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
                                                                cancellationToken);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits live location of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Message?> Edit(this IContainer<CallbackQuery> simpleContext,
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
                await simpleContext.BotClient.EditMessageLiveLocationAsync(simpleContext.Update.InlineMessageId,
                                                                        latitude,
                                                                        longitude,
                                                                        horizontalAccuracy,
                                                                        heading,
                                                                        proximityAlertRadius,
                                                                        inlineKeyboardMarkup,
                                                                        cancellationToken);
                return null;
            }
            else if (simpleContext.Update.Message != null)
            {
                return await simpleContext.BotClient.EditMessageLiveLocationAsync(simpleContext.Update.Message.Chat.Id,
                                                                               simpleContext.Update.Message.MessageId,
                                                                               latitude,
                                                                               longitude,
                                                                               horizontalAccuracy,
                                                                               heading,
                                                                               proximityAlertRadius,
                                                                               inlineKeyboardMarkup,
                                                                               cancellationToken);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits media of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Message?> Edit(this IContainer<CallbackQuery> simpleContext,
                                                InputMediaBase inputMediaBase,
                                                InlineKeyboardMarkup? inlineKeyboardMarkup = default,
                                                CancellationToken cancellationToken = default)
        {
            if (simpleContext.Update.InlineMessageId != null)
            {
                await simpleContext.BotClient.EditMessageMediaAsync(simpleContext.Update.InlineMessageId,
                                                                inputMediaBase,
                                                                inlineKeyboardMarkup,
                                                                cancellationToken);
                return null;
            }
            else if (simpleContext.Update.Message != null)
            {
                return await simpleContext.BotClient.EditMessageMediaAsync(simpleContext.Update.Message.Chat.Id,
                                                                simpleContext.Update.Message.MessageId,
                                                                inputMediaBase,
                                                                inlineKeyboardMarkup,
                                                                cancellationToken);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits caption of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Message?> Edit(this IContainer<CallbackQuery> simpleContext,
                                                string caption,
                                                ParseMode? parseMode = default,
                                                IEnumerable<MessageEntity>? messageEntities = default,
                                                InlineKeyboardMarkup? inlineKeyboardMarkup = default,
                                                CancellationToken cancellationToken = default)
        {
            if (simpleContext.Update.InlineMessageId != null)
            {
                await simpleContext.BotClient.EditMessageCaptionAsync(simpleContext.Update.InlineMessageId,
                                                                caption,
                                                                parseMode,
                                                                messageEntities,
                                                                inlineKeyboardMarkup,
                                                                cancellationToken);
                return null;
            }
            else if (simpleContext.Update.Message != null)
            {
                return await simpleContext.BotClient.EditMessageCaptionAsync(simpleContext.Update.Message.Chat.Id,
                                                                simpleContext.Update.Message.MessageId,
                                                                caption,
                                                                parseMode,
                                                                messageEntities,
                                                                inlineKeyboardMarkup,
                                                                cancellationToken);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits reply markup of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<Message?> Edit(this IContainer<CallbackQuery> simpleContext,
                                                InlineKeyboardMarkup? inlineKeyboardMarkup = default,
                                                CancellationToken cancellationToken = default)
        {
            if (simpleContext.Update.InlineMessageId != null)
            {
                await simpleContext.BotClient.EditMessageReplyMarkupAsync(simpleContext.Update.InlineMessageId,
                                                                       inlineKeyboardMarkup,
                                                                       cancellationToken);
                return null;
            }
            else if (simpleContext.Update.Message != null)
            {
                return await simpleContext.BotClient.EditMessageReplyMarkupAsync(simpleContext.Update.Message.Chat.Id,
                                                                              simpleContext.Update.Message.MessageId,
                                                                              inlineKeyboardMarkup,
                                                                              cancellationToken);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }
    }
}
