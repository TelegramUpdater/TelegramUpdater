using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace UpdaterProduction
{
    internal class MyScopedMessageHandler : ScopedMessageHandler
    {
        protected override async Task HandleAsync(UpdateContainerAbs<Message> updateContainer)
        {
            await updateContainer.Response($"Scoped! {GetHashCode()}");
        }
    }
}
