using System;
using System.Linq;
using Telegram.Bot.Types;

namespace TelegramUpdater.Filters
{
    internal class FromUsernamesFilter<T> : Filter<T> where T: class
    {
        internal FromUsernamesFilter(
            Func<T, string?> usernameSelector, params string[] usernames)
            : base(x =>
            {
                var username = usernameSelector(x);
                if (username == null) return false;

                return usernames.Any(x => x == username);
            })
        {
        }
    }

    /// <summary>
    /// Use this to create a <see cref="FromUsernamesFilter{T}"/>. where is Update type.
    /// </summary>
    public static class FromUsernamesFilter
    {
        /// <summary>
        /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="Message"/> handlers
        /// </summary>
        /// <param name="usernames">A list of usernames without @.</param>
        public static Filter<Message> Messages(params string[] usernames)
            => new FromUsernamesFilter<Message>(x => x.From?.Username, usernames);

        /// <summary>
        /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="CallbackQuery"/> handlers
        /// </summary>
        /// <param name="usernames">A list of usernames without @.</param>
        public static Filter<CallbackQuery> CallbackQueries(params string[] usernames)
            => new FromUsernamesFilter<CallbackQuery>(x => x.From.Username, usernames);

        /// <summary>
        /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="InlineQuery"/> handlers
        /// </summary>
        /// <param name="usernames">A list of usernames without @.</param>
        public static Filter<InlineQuery> InlineQueries(params string[] usernames)
            => new FromUsernamesFilter<InlineQuery>(x => x.From.Username, usernames);
    }
}
