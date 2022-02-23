using System;
using System.Linq;
using Telegram.Bot.Types;

namespace TelegramUpdater.Filters
{
    internal class FromUsersFilter<T> : Filter<T>
    {
        internal FromUsersFilter(Func<T, long?> userSelector, params long[] users)
            : base(x =>
            {
                var user = userSelector(x);
                if (user is null) return false;

                return users.Any(x => x == user);
            })
        {
        }
    }

    /// <summary>
    /// Use this to create a <see cref="FromUsersFilter{T}"/>. where is Update type.
    /// </summary>
    public static class FromUsersFilter
    {
        /// <summary>
        /// Create an instance of <see cref="FromUsersFilter{T}"/> for <see cref="Message"/> handlers.
        /// </summary>
        /// <param name="users">User ids</param>
        public static Filter<Message> Messages(params long[] users)
            => new FromUsersFilter<Message>(x => x.From?.Id, users);

        /// <summary>
        /// Create an instance of <see cref="FromUsersFilter{T}"/> for <see cref="CallbackQuery"/> handlers.
        /// </summary>
        /// <param name="users">User ids</param>
        public static Filter<CallbackQuery> CallbackQueries(params long[] users)
            => new FromUsersFilter<CallbackQuery>(x => x.From.Id, users);

        /// <summary>
        /// Create an instance of <see cref="FromUsersFilter{T}"/> for <see cref="InlineQuery"/> handlers.
        /// </summary>
        /// <param name="users">User ids</param>
        public static Filter<InlineQuery> InlineQueries(params long[] users)
            => new FromUsersFilter<InlineQuery>(x => x.From.Id, users);
    }
}
