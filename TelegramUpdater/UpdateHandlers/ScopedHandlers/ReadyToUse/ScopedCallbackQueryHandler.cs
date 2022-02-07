using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    public abstract class ScopedCallbackQueryHandler : AnyScopedHandler<CallbackQuery>
    {
        protected ScopedCallbackQueryHandler(int group = 0)
            : base(x => x.CallbackQuery, group)
        {
        }
    }
}
