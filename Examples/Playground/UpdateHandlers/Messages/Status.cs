using Microsoft.EntityFrameworkCore;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.Attributes;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("status"), Private]
[ScopedHandler(Group = 0, LayerId = "StatusLayer")]
internal class StatusFilter(PlaygroundMemory memory) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        if (From == null)
            StopPropagation();
        
        if (!await memory.SeenUsers.AnyAsync(x=> x.TelegramId == From.Id))
        {
            StopPropagation();
        }

        // If there's no user or, the user has not been seen stop handling
        // reaming handlers in this layer
    }
}

[Command("status"), Private]
[ScopedHandler(Group = 1, LayerId = "StatusLayer")]
internal class StatusSeen : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        await Response("Hooray! you have been seen");
    }
}

//[Command("status"), Private]
//// Default group and layer
//internal class StatusNotSeen : MessageHandler
//{
//    protected override async Task HandleAsync(MessageContainer container)
//    {
//        await Response("Oh! i have not seen you yet");
//    }
//}
