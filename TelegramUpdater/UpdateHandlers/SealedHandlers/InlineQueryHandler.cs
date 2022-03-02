using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    /// <summary>
    /// An update handler for <see cref="Update.InlineQuery"/>.
    /// </summary>
    public sealed class InlineQueryHandler : AnyUpdateHandler<InlineQuery>
    {
        /// <summary>
        /// Create update handler for <see cref="InlineQuery"/>.
        /// </summary>
        /// <param name="filter">Filters for this handler.</param>
        /// <param name="group">Handling priority for this handler.</param>
        /// <param name="callback">
        /// A callback function where you may handle the incoming update.
        /// </param>
        public InlineQueryHandler(Func<IContainer<InlineQuery>, Task> callback,
                                  IFilter<InlineQuery>? filter = default,
                                  int group = 0)
            : base(UpdateType.InlineQuery,
                   x => x.InlineQuery,
                   callback,
                   filter,
                   group)
        { }
    }
}
