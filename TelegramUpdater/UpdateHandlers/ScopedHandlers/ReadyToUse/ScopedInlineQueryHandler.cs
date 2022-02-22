using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    public abstract class ScopedInlineQueryHandler : AnyScopedHandler<InlineQuery>
    {
        protected ScopedInlineQueryHandler(int group = default) : base(x => x.InlineQuery, group)
        {
        }
    }
}
