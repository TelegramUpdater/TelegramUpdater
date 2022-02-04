using Telegram.Bot.Types;

namespace TelegramUpdater.Filters
{
    /// <summary>
    /// A filter on <see cref="CallbackQuery.Data"/>
    /// </summary>
    public class CallbackQueryDataFilter : Filter<CallbackQuery>
    {
        /// <summary>
        /// A filter on <see cref="CallbackQuery.Data"/>
        /// </summary>
        public CallbackQueryDataFilter(Func<string, bool> filter)
            : base((x) => x.Data is not null && filter(x.Data))
        { }
    }
}
