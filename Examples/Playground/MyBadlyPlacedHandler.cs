using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground;

[Command("bad"), Private]
internal class MyBadlyPlacedHandler : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        await Response("This is a badly placed handler!");
    }
}
