using System;
using System.Linq;
using Telegram.Bot.Types;

namespace TelegramUpdater.Filters
{
    public class FromUsersFilter<T> : Filter<T>
    {
        public FromUsersFilter(Func<T, long?> userSelector, params long[] users)
            : base(x =>
            {
                var user = userSelector(x);
                if (user is null) return false;

                return users.Any(x => x == user);
            })
        {
        }
    }

    public class FromUsersMessageFilter : FromUsersFilter<Message>
    {
        public FromUsersMessageFilter(params long[] users)
            : base(x=> x.From?.Id, users)
        {
        }
    }

    public class FromUsersCallbackQueryFilter : FromUsersFilter<CallbackQuery>
    {
        public FromUsersCallbackQueryFilter(params long[] users)
            : base(x => x.From?.Id, users)
        {
        }
    }
}
