using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
            Filter<Message>? filter = default,
            int group = 0)
            : base(UpdateType.Message, x => x.Message, callbak, filter, group)
        {
        }
    }
}
