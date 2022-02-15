using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    /// <summary>
    /// Abstarct <see cref="IScopedUpdateHandler"/> for <see cref="Update.CallbackQuery"/>.
    /// </summary>
    public abstract class ScopedCallbackQueryHandler : AnyScopedHandler<CallbackQuery>
    {
        protected ScopedCallbackQueryHandler(int group = 0)
            : base(x => x.CallbackQuery, group)
        {
        }
    }
}
