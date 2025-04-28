// Ignore Spelling: cntr

using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("start"), Private]
internal class Start(PlaygroundMemory memory) : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> cntr)
    {
        if (From is null) return;

        var known = await memory.SeenUsers
            .AnyAsync(x => x.TelegramId == From.Id);

        if (known)
            await Response("Welcome back my friend!");
        else
        {
            memory.SeenUsers.Add(new()
            {
                TelegramId = From.Id,
                Name = From.FirstName,
            });

            await memory.SaveChangesAsync();

            await Response("Hello traveler!");
        }
    }
}
