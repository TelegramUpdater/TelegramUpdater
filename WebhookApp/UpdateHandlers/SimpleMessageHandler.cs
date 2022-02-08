using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace WebhookApp.UpdateHandlers
{
    [ApplyFilter(typeof(PrivateHelloCommand))]
    public class SimpleMessageHandler : ScopedMessageHandler
    {
        protected override async Task HandleAsync(UpdateContainerAbs<Message> updateContainer)
        {
            await updateContainer.Response("Hello World");
        }
    }

    internal class PrivateHelloCommand : Filter<Message>
    {
        public PrivateHelloCommand()
            : base(FilterCutify.OnCommand("hello") & FilterCutify.PM())
        {
        }
    }
}
