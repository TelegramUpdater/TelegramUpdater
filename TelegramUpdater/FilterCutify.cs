using System;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.Filters;

namespace TelegramUpdater
{
    /// <summary>
    /// A collection of cutified filters for kids.
    /// </summary>
    public static class FilterCutify
    {
        /// <summary>
        /// The handler will always be triggered on specified update type of <typeparamref name="T"/>
        /// </summary>
        public static Filter<T> Always<T>() => new Filter<T>((_) => true);

        /// <summary>
        /// The handler will be triggered when <paramref name="func"/> returns true
        /// on specified update type of <typeparamref name="T"/>
        /// </summary>
        public static Filter<T> When<T>(Func<T, bool> func) => new Filter<T>(func);

        /// <summary>
        /// The handler will be triggered when <paramref name="func"/> passes
        /// on specified update type of <typeparamref name="T"/>
        /// </summary>
        public static Filter<T>? When<T>(Filter<T>? func) => func;

        /// <summary>
        /// The handler will be triggered when a message is a command specified in <paramref name="commands"/>
        /// </summary>
        public static Filter<Message> OnCommand(params string[] commands)
            => new CommandFilter(commands);

        /// <summary>
        /// The handler will be triggered when a message is a command specified in <paramref name="commands"/>
        /// </summary>
        public static Filter<Message> OnCommand(char prefix = '/', params string[] commands)
            => new CommandFilter(prefix, commands);

        /// <summary>
        /// The handler will be triggered when a regex matchs its text.
        /// </summary>
        public static Filter<Message> TextMatchs(
            string pattern, bool catchCaption = false, RegexOptions? regexOptions = default)
                => new MessageTextRegex(pattern, catchCaption, regexOptions);

        /// <summary>
        /// The handler will be triggered when a regex matchs its data.
        /// </summary>
        public static Filter<CallbackQuery> DataMatches(
            string pattern, RegexOptions? regexOptions = default)
            => new CallbackQueryRegex(pattern, regexOptions);

        /// <summary>
        /// The handler will be triggered when a message sent in private chat
        /// </summary>
        public static Filter<Message> PM() => new PrivateMessageFilter();

        /// <summary>
        /// Filter messages from <see cref="ChatType.Group"/> and <see cref="ChatType.Supergroup"/>.
        /// </summary>
        /// <returns></returns>
        public static Filter<Message> Group() => new Filter<Message>(
            x => x.Chat.Type == ChatType.Group || x.Chat.Type == ChatType.Supergroup);

        /// <summary>
        /// A message comes only from specified user(s) is <paramref name="users"/>
        /// </summary>
        public static Filter<Message> MsgOfUsers(params long[] users)
            => FromUsersFilter.Messages(users);

        /// <summary>
        /// A callback query comes only from specified user(s) is <paramref name="users"/>
        /// </summary>
        public static Filter<CallbackQuery> CbqOfUsers(params long[] users)
            => FromUsersFilter.CallbackQueries(users);

        /// <summary>
        /// A replied message.
        /// </summary>
        public static Filter<Message> Replied() => new MessageRepliedFilter();

        /// <summary>
        /// Creates a filter that checks if a selected property is not null.
        /// </summary>
        /// <param name="selector">Function to select a property out of <typeparamref name="T"/></param>
        /// <returns></returns>
        public static Filter<K> NotNullFilter<K, T>(Func<K, T?> selector)
            where T : class
            => new Filter<K>(x => selector(x) != null);

        /// <summary>
        /// Filters text messages.
        /// </summary>
        public static Filter<Message> Text()
            => NotNullFilter<Message, string>(x => x.Text);

        /// <summary>
        /// Filters messages with caption.
        /// </summary>
        public static Filter<Message> Caption()
            => NotNullFilter<Message, string>(x => x.Caption);

        /// <summary>
        /// Filters photo messages.
        /// </summary>
        public static Filter<Message> Photo()
            => NotNullFilter<Message, PhotoSize[]>(x => x.Photo);

        /// <summary>
        /// Filters video messages.
        /// </summary>
        public static Filter<Message> Video()
            => NotNullFilter<Message, Video>(x => x.Video);
    }
}
