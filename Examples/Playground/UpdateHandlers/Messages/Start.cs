// Ignore Spelling: cntr

using Microsoft.EntityFrameworkCore;
using Playground.Models;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("start"), Private]
internal class Start(PlaygroundMemory memory) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer cntr)
    {
        if (From is null) return;

        // Ignore if user is in rename state
        DeleteState<RenameState>(From);

        var knownUser = await memory.SeenUsers
            .SingleOrDefaultAsync(x => x.TelegramId == From.Id);

        if (knownUser is SeenUser seen)
            await Response($"Welcome back my friend {seen.Name}!");
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
