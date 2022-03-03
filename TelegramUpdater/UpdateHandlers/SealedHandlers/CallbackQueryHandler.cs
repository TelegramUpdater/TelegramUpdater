using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    /// <summary>
    /// An update handler for <see cref="Update.CallbackQuery"/>.
    /// </summary>
    public sealed class CallbackQueryHandler : AnyUpdateHandler<CallbackQuery>
    {
        /// <summary>
        /// Create update handler for <see cref="CallbackQuery"/>.
        /// </summary>
        /// <param name="filter">Filters for this handler.</param>
        /// <param name="group">Handling priority for this handler.</param>
        /// <param name="callback">
        /// A callback function where you may handle the incoming update.
        /// </param>
        public CallbackQueryHandler(
            Func<IContainer<CallbackQuery>, Task> callback,
            IFilter<CallbackQuery>? filter = default,
            int group = 0)
            : base(UpdateType.CallbackQuery,
                   x => x.CallbackQuery,
                   callback,
                   filter,
                   group)
        { }
    }
}
