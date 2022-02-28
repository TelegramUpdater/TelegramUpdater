using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    /// <summary>
    /// An update handler for <see cref="Update.Message"/>.
    /// </summary>
    public sealed class MessageHandler : AnyUpdateHandler<Message>
    {
        public MessageHandler(
            Func<IContainer<Message>, Task> callbak,
            IFilter<Message>? filter = default,
            int group = 0)
            : base(UpdateType.Message, x => x.Message, callbak, filter, group)
        {
        }
    }
}
