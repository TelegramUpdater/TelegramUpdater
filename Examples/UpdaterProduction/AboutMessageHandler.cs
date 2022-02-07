using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace UpdaterProduction
{
    [ApplyFilter(typeof(AboutCommand))]
    internal class AboutMessageHandler : ScopedMessageHandler
    {
        protected override async Task HandleAsync(UpdateContainerAbs<Message> container)
        {
            await container.Response($"How about you?");
        }
    }

    internal class AboutCommand : Filter<Message>
    {
        public AboutCommand()
            : base(FilterCutify.OnCommand("about") & FilterCutify.PM())
        {
        }
    }
}
