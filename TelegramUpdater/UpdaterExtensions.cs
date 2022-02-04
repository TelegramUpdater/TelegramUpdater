using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateChannels.SealedChannels;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater
{
    public static class CommonExtensions
    {
        internal static MessageContainer RebaseContainer<T>(
            this UpdateContainerAbs<T> containerAbs, Message message) where T : class
            => new(
                containerAbs.Updater,
                new Update { Message = message },
                containerAbs.BotClient);

        internal static MessageContainer Wrap<T>(this Message message,
                                            UpdateContainerAbs<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(message);

        internal static async Task<MessageContainer> WrapAsync<T>(this Task<Message> message,
                                            UpdateContainerAbs<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(await message);

        internal static CallbackQueryContainer RebaseContainer<T>(
            this UpdateContainerAbs<T> containerAbs, CallbackQuery callbackQuery) where T : class
            => new(
                containerAbs.Updater,
                new Update { CallbackQuery = callbackQuery },
                containerAbs.BotClient);

        internal static CallbackQueryContainer Wrap<T>(this CallbackQuery message,
                                            UpdateContainerAbs<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(message);

        internal static async Task<CallbackQueryContainer> WrapAsync<T>(this Task<CallbackQuery> message,
                                            UpdateContainerAbs<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(await message);

        public static object GetInnerUpdate(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}");
        }

        public static T GetInnerUpdate<T>(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return (T)(typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}"));
        }

        public static UpdateType? GetUpdateType<T>()
        {
            if (Enum.TryParse(typeof(T).Name, out UpdateType result))
            {
                return result;
            }

            return null;
        }
    }

    public static class PropagationExtensions
    {
        /// <summary>
        /// All pending handlers for this update will be ignored after throwing this.
        /// </summary>
        public static void StopPropagation<T>(this UpdateContainerAbs<T> _)
            where T : class => throw new StopPropagation();

        /// <summary>
        /// Continue to the next pending handler for this update and ignore the rest of this handler.
        /// </summary>
        public static void ContinuePropagation<T>(this UpdateContainerAbs<T> _)
            where T : class => throw new ContinuePropagation();
    }

    public static class UpdateExtensions
    {
        /// <summary>
        /// Gets sender id of an update. ( usually .From or .SenderChat )
        /// </summary>
        public static long? GetSenderId(this Update update)
        {
            return update switch
            {
                { Message: { From: { } from } } => from.Id,
                { Message: { SenderChat: { } chat } } => chat.Id,
                { EditedMessage: { From: { } from } } => from.Id,
                { EditedMessage: { SenderChat: { } chat } } => chat.Id,
                { ChannelPost: { From: { } from } } => from.Id,
                { ChannelPost: { SenderChat: { } chat } } => chat.Id,
                { EditedChannelPost: { From: { } from } } => from.Id,
                { EditedChannelPost: { SenderChat: { } chat } } => chat.Id,
                { CallbackQuery: { } call } => call.From.Id,
                { InlineQuery: { From: { } from } } => from.Id,
                { PollAnswer: { User : { } user } } => user.Id,
                { PreCheckoutQuery: { From: { } from } } => from.Id,
                { ShippingQuery: { From: { } from } } => from.Id,
                { ChosenInlineResult: { From: { } from } } => from.Id,
                { ChatJoinRequest: { From: { } from } } => from.Id,
                { ChatMember: { From: { } from } } => from.Id,
                { MyChatMember: { From: { } from } } => from.Id,
                _ => null
            };
        }
    }

    public static class ChannelsExtensions
    {
        /// <summary>
        /// Opens a channel that dispatches a <see cref="Message"/> from updater.
        /// </summary>
        /// <param name="updateContainer">The update container</param>
        /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
        /// <param name="filter">Filter updates to get the right one.</param>
        public static async Task<UpdateContainerAbs<Message>?> ChannelMessage(this UpdateContainerAbs<Message> updateContainer,
                                                          Filter<Message>? filter,
                                                          TimeSpan? timeOut = default)
        {
            var message = await updateContainer.OpenChannel(
                  new MessageChannel(filter), timeOut ?? TimeSpan.FromSeconds(30));
            if (message is not null)
                return message.Wrap(updateContainer);
            return null;
        }


        /// <summary>
        /// Opens a channel that dispatches a <see cref="Message"/> of this user from updater.
        /// </summary>
        /// <param name="updateContainer">The update container</param>
        /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
        /// <param name="filter">Filter updates to get the right one.</param>
        public static async Task<UpdateContainerAbs<Message>?> ChannelUserResponse(this UpdateContainerAbs<Message> updateContainer,
                                                          Filter<Message>? filter = default,
                                                          TimeSpan? timeOut = default)
        {
            if (updateContainer.Update.From is not null)
            {
                var realFilter = FilterCutify.MsgOfUsers(updateContainer.Update.From.Id);
                if (filter != null)
                {
                    realFilter &= filter;
                }

                return await updateContainer.ChannelMessage(realFilter, timeOut);
            }
            else
            {
                throw new InvalidOperationException("Sender can't be null!");
            }
        }

        public static async Task<UpdateContainerAbs<CallbackQuery>?> ChannelUserClick<T>(this UpdateContainerAbs<T> updateContainer,
                                                                     Func<T, long?> senderIdResolver,
                                                                     TimeSpan timeOut,
                                                                     string pattern,
                                                                     RegexOptions? regexOptions = default) where T : class
        {
            var senderId = senderIdResolver(updateContainer.Update);
            if (senderId is not null)
            {
                var result = await updateContainer.Updater.OpenChannel(
                    new CallbackQueryChannel(
                        FilterCutify.CbqOfUsers(senderId.Value) &
                        FilterCutify.DataMatches(pattern, regexOptions)),
                    timeOut);

                if (result != null)
                    return result.Wrap(updateContainer);
                return null;
            }
            else
            {
                throw new InvalidOperationException("Sender can't be null!");
            }
        }

        public static async Task<UpdateContainerAbs<CallbackQuery>?> ChannelUserClick(this UpdateContainerAbs<Message> updateContainer,
                                                                  TimeSpan timeOut,
                                                                  string pattern,
                                                                  RegexOptions? regexOptions = default)
        {
            return await updateContainer.ChannelUserClick(x => x.From?.Id ?? x.SenderChat?.Id ?? null,
                                                          timeOut,
                                                          pattern,
                                                          regexOptions);
        }

        public static async Task<UpdateContainerAbs<CallbackQuery>?> ChannelUserClick(this UpdateContainerAbs<CallbackQuery> updateContainer,
                                                                  TimeSpan timeOut,
                                                                  string pattern,
                                                                  RegexOptions? regexOptions = default)
        {
            return await updateContainer.ChannelUserClick(x => x.From.Id,
                                                          timeOut,
                                                          pattern,
                                                          regexOptions);
        }
    }

    public static class CallbackQueryContextExtensions
    {
        public static long SenderId(this UpdateContainerAbs<CallbackQuery> simpleContext)
            => simpleContext.Update.From.Id;

        public static User Sender(this UpdateContainerAbs<CallbackQuery> simpleContext)
            => simpleContext.Update.From;

        public static async Task<UpdateContainerAbs<Message>> Send(this UpdateContainerAbs<CallbackQuery> simpleContext,
                                                string text,
                                                bool sendAsReply = true,
                                                ParseMode? parseMode = default,
                                                IEnumerable<MessageEntity>? messageEntities = default,
                                                bool? disableWebpagePreview = default,
                                                bool? disableNotification = default,
                                                IReplyMarkup? replyMarkup = default)
        {
            if (simpleContext.Update.Message is not null)
            {

                return await simpleContext.BotClient.SendTextMessageAsync(simpleContext.Update.Message.Chat.Id,
                                                                       text,
                                                                       parseMode,
                                                                       messageEntities,
                                                                       disableWebpagePreview,
                                                                       disableNotification,
                                                                       replyToMessageId: sendAsReply ? simpleContext.Update.Message.MessageId : 0,
                                                                       allowSendingWithoutReply: true,
                                                                       replyMarkup: replyMarkup)
                    .WrapAsync(simpleContext);
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
        public static async Task Answer(this UpdateContainerAbs<CallbackQuery> simpleContext, string? text = default,
                                        bool? showAlert = default, string? url = default, int? cacheTime = default,
                                        CancellationToken cancellationToken = default)
            => await simpleContext.BotClient.AnswerCallbackQueryAsync(simpleContext.Update.Id, text, showAlert, url,
                                                                   cacheTime, cancellationToken);

        /// <summary>
        /// Edits a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<UpdateContainerAbs<Message>?> Edit(this UpdateContainerAbs<CallbackQuery> simpleContext,
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
                    .WrapAsync(simpleContext);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits live location of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<UpdateContainerAbs<Message>?> Edit(this UpdateContainerAbs<CallbackQuery> simpleContext,
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
                                                                               cancellationToken)
                    .WrapAsync(simpleContext);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits media of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<UpdateContainerAbs<Message>?> Edit(this UpdateContainerAbs<CallbackQuery> simpleContext,
                                                string text,
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
                                                                cancellationToken)
                    .WrapAsync(simpleContext);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits caption of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<UpdateContainerAbs<Message>?> Edit(this UpdateContainerAbs<CallbackQuery> simpleContext,
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
                                                                cancellationToken)
                    .WrapAsync(simpleContext);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }

        /// <summary>
        /// Edits reply markup of a message
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<UpdateContainerAbs<Message>?> Edit(this UpdateContainerAbs<CallbackQuery> simpleContext,
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
                                                                              cancellationToken)
                    .WrapAsync(simpleContext);
            }

            throw new InvalidOperationException("InlineMessageId and Message are both null!");
        }
    }

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

    public static class ConditionalExtensions
    {
        #region Sync

        #region Abstracts
        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static MatchContext<T> If<T>(this UpdateContainerAbs<T> simpleContext,
                                            Func<T, string?> getText,
                                            string pattern,
                                            Action<UpdateContainerAbs<T>> func,
                                            RegexOptions? regexOptions = default) where T : class
        {
            var match = MatchContext<T>.Check(simpleContext, getText, pattern, regexOptions);

            if (match)
            {
                func(simpleContext);
            }

            return match;
        }

        /// <summary>
        /// Do something when a condition is true.
        /// </summary>
        public static MatchContext<T> If<T>(this UpdateContainerAbs<T> simpleContext,
                                            Func<UpdateContainerAbs<T>, bool> predict,
                                            Action<UpdateContainerAbs<T>> func) where T : class
        {
            if (predict(simpleContext))
            {
                func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        /// <summary>
        /// Do something when a regex not matched.
        /// </summary>
        public static void Else<T>(this MatchContext<T> matchContext,
                                   Action<UpdateContainerAbs<T>> func) where T : class
        {
            var match = matchContext;
            if (!match)
            {
                func(match.SimpleContext);
            }
        }

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static MatchContext<T> ElseIf<T>(this MatchContext<T> matchContext,
                                                Func<T, string?> getText,
                                                string pattern,
                                                Action<UpdateContainerAbs<T>> func,
                                                RegexOptions? regexOptions = default) where T : class
        {
            if (!matchContext)
            {
                var match = MatchContext<T>.Check(matchContext.SimpleContext, getText, pattern, regexOptions);

                if (match)
                {
                    func(matchContext.SimpleContext);
                }

                return match;
            }

            return matchContext;
        }

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static MatchContext<T> ElseIf<T>(this MatchContext<T> matchContext,
                                                Func<UpdateContainerAbs<T>, bool> predict,
                                                Action<UpdateContainerAbs<T>> func) where T : class
        {
            if (!matchContext)
            {
                if (predict(matchContext.SimpleContext))
                {
                    func(matchContext.SimpleContext);
                    return new(matchContext.SimpleContext, true);
                }

                return default;
            }

            return matchContext;
        }

        public static MatchContext<T> IfNotNull<T>(this UpdateContainerAbs<T>? simpleContext,
                                                   Action<UpdateContainerAbs<T>> func) where T : class
        {
            if (simpleContext is not null)
            {
                func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        public static void IfNotNull<T>(this T everything, Action<T> action)
            where T : class
        {
            if (everything is not null)
            {
                action(everything);
            }
        }

        #endregion

        #endregion

        #region Async

        #region Abstracts
        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this UpdateContainerAbs<T> simpleContext,
                                                        Func<T, string?> getText,
                                                        string pattern,
                                                        Func<UpdateContainerAbs<T>, Task> func,
                                                        RegexOptions? regexOptions = default) where T : class
        {
            var match = MatchContext<T>.Check(simpleContext, getText, pattern, regexOptions);

            if (match)
            {
                await func(simpleContext);
            }

            return match;
        }

        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this Task<UpdateContainerAbs<T>> simpleContext,
                                                        Func<T, string?> getText,
                                                        string pattern,
                                                        Func<UpdateContainerAbs<T>, Task> func,
                                                        RegexOptions? regexOptions = default) where T : class
        {
            var gottenContext = await simpleContext;

            var match = MatchContext<T>.Check(gottenContext, getText, pattern, regexOptions);

            if (match)
            {
                await func(gottenContext);
            }

            return match;
        }

        /// <summary>
        /// Do something when a condition is true.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this UpdateContainerAbs<T> simpleContext,
                                                        Func<UpdateContainerAbs<T>, bool> predict,
                                                        Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            if (predict(simpleContext))
            {
                await func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        /// <summary>
        /// Do something when a condition is true.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this Task<UpdateContainerAbs<T>> simpleContext,
                                                        Func<UpdateContainerAbs<T>, bool> predict,
                                                        Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var gottenContext = await simpleContext;

            if (predict(gottenContext))
            {
                await func(gottenContext);
                return new MatchContext<T>(gottenContext, true);
            }

            return default;
        }

        /// <summary>
        /// If this <see cref="UpdateContainerAbs{T}"/> is not null
        /// </summary>
        public static async Task<MatchContext<T>> IfNotNull<T>(this UpdateContainerAbs<T>? simpleContext,
                                                               Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            if (simpleContext is not null)
            {
                await func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        /// <summary>
        /// If this <see cref="UpdateContainerAbs{T}"/> is not null
        /// </summary>
        public static async Task<MatchContext<T>> IfNotNull<T>(this Task<UpdateContainerAbs<T>?> simpleContext,
                                                               Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var gottenContext = await simpleContext;

            if (gottenContext is not null)
            {
                await func(gottenContext);
                return new MatchContext<T>(gottenContext, true);
            }

            return default;
        }

        /// <summary>
        /// Do something when a regex not matched.
        /// </summary>
        public static async Task Else<T>(this Task<MatchContext<T>> matchContext,
                                         Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var match = await matchContext;
            if (!match)
            {
                await func(match.SimpleContext);
            }
        }

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<T>> ElseIf<T>(this Task<MatchContext<T>> matchContext,
                                                            Func<T, string?> getText,
                                                            string pattern,
                                                            Func<UpdateContainerAbs<T>, Task> func,
                                                            RegexOptions? regexOptions = default) where T : class
        {
            var prevMatch = await matchContext;
            if (!prevMatch)
            {
                var match = MatchContext<T>.Check(prevMatch.SimpleContext, getText, pattern, regexOptions);

                if (match)
                {
                    await func(prevMatch.SimpleContext);
                }

                return match;
            }

            return prevMatch;
        }

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<T>> ElseIf<T>(this Task<MatchContext<T>> matchContext,
                                                            Func<UpdateContainerAbs<T>, bool> predict,
                                                            Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var prevMatch = await matchContext;
            if (!prevMatch)
            {
                if (predict(prevMatch.SimpleContext))
                {
                    await func(prevMatch.SimpleContext);
                    return new(prevMatch.SimpleContext, true);
                }

                return default;
            }

            return prevMatch;
        }

        #endregion

        #region Sealed

        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<Message>> If(this UpdateContainerAbs<Message> simpleContext,
                                                                  string pattern,
                                                                  Func<UpdateContainerAbs<Message>, Task> func,
                                                                  RegexOptions? regexOptions = default)
            => await simpleContext.If(x => x.Text, pattern, func, regexOptions);

        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<CallbackQuery>> If(this UpdateContainerAbs<CallbackQuery> simpleContext,
                                                                        string pattern,
                                                                        Func<UpdateContainerAbs<CallbackQuery>, Task> func,
                                                                        RegexOptions? regexOptions = default)
            => await simpleContext.If(x => x.Data, pattern, func, regexOptions);

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<CallbackQuery>> ElseIf(this Task<MatchContext<CallbackQuery>> matchContext,
                                                                     string pattern,
                                                                     Func<UpdateContainerAbs<CallbackQuery>, Task> func,
                                                                     RegexOptions? regexOptions = default)
            => await matchContext.ElseIf(x => x.Data, pattern, func, regexOptions);

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<Message>> ElseIf(this Task<MatchContext<Message>> matchContext,
                                                               string pattern,
                                                               Func<UpdateContainerAbs<Message>, Task> func,
                                                               RegexOptions? regexOptions = default)
            => await matchContext.ElseIf(x => x.Text, pattern, func, regexOptions);

        #endregion

        #endregion
    }

}
