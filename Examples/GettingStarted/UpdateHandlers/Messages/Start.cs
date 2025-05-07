using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace GettingStarted.UpdateHandlers.Messages;

[Command("start"), Private]
internal class Start : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        await container.Response("Hello World");
    }
}
