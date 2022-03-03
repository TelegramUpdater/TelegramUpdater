namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    /// <summary>
    /// Abstract <see cref="IScopedUpdateHandler"/> for
    /// <see cref="Update.CallbackQuery"/>.
    /// </summary>
    public abstract class ScopedCallbackQueryHandler
        : AnyScopedHandler<CallbackQuery>
    {
        /// <summary>
        /// You can set handling priority in here.
        /// </summary>
        /// <param name="group">Handling priority.</param>
        protected ScopedCallbackQueryHandler(int group = 0)
            : base(x => x.CallbackQuery, group)
        {
        }
    }
}
