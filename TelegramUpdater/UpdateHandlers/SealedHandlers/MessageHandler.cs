using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.AbstractHandlers;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public sealed class MessageHandler : MessageHandlerAbs
    {
        private readonly Filter<Message>? _filter;
        private readonly Func<UpdateContainerAbs<Message>, Task> _handleAsync;

        public MessageHandler(
            Func<UpdateContainerAbs<Message>, Task> handleAsync,
            Filter<Message>? filter = default,
            int group = 0): base(group)
        {
            _filter = filter;
            _handleAsync = handleAsync;
        }

        protected override async Task HandleAsync(UpdateContainerAbs<Message> updateContainer)
            => await _handleAsync(updateContainer);

        protected override bool ShouldHandle(Message t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }
    }
}
