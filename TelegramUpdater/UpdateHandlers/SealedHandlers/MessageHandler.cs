using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public sealed class MessageHandler : AnyUpdateHandler<Message>
    {
        public MessageHandler(
            Func<UpdateContainerAbs<Message>, Task> callbak,
            Filter<Message>? filter,
            int group = 0)
            : base(UpdateType.Message, x => x.Message, callbak, filter, group)
        {
        }
    }
}
