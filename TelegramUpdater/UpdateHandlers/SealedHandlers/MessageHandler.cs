using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.AbstractHandlers;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public sealed class MessageHandler : MessageHandlerAbs
    {
        private readonly Func<UpdateContainerAbs<Message>, Task> _handleAsync;

        public MessageHandler(
            Func<UpdateContainerAbs<Message>, Task> handleAsync,
            Filter<Message>? filter = default,
            int group = 0): base(filter, group)
        {
            _handleAsync = handleAsync;
        }

        protected override async Task HandleAsync(UpdateContainerAbs<Message> updateContainer)
            => await _handleAsync(updateContainer);
    }
}
