using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    /// <summary>
    /// An update handler for <see cref="Update.InlineQuery"/>.
    /// </summary>
    public sealed class InlineQueryHandler : AnyUpdateHandler<InlineQuery>
    {
        public InlineQueryHandler(Func<IContainer<InlineQuery>, Task> callbak,
                                  Filter<InlineQuery>? filter = default,
                                  int group = 0)
            : base(UpdateType.InlineQuery, x => x.InlineQuery, callbak, filter, group)
        {
        }
    }
}
