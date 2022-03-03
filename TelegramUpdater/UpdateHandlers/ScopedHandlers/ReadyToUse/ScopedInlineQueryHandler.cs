namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    /// <summary>
    /// Abstract <see cref="IScopedUpdateHandler"/> for
    /// <see cref="Update.InlineQuery"/>.
    /// </summary>
    public abstract class ScopedInlineQueryHandler
        : AnyScopedHandler<InlineQuery>
    {
        /// <summary>
        /// You can set handling priority in here.
        /// </summary>
        /// <param name="group">Handling priority.</param>
        protected ScopedInlineQueryHandler(int group = default)
            : base(x => x.InlineQuery, group)
        {
        }
    }
}
