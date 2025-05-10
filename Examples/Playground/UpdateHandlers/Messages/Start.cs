// Ignore Spelling: cntr

using Microsoft.EntityFrameworkCore;
using Playground.Models;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Filters;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("start", argumentsMode: ArgumentsMode.NoArgs), Private]
internal class Start(PlaygroundMemory memory) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer cntr)
    {
        if (From is null) return;

        var deeplink = await Updater.CreateDeeplink("restart");
        var button = InlineKeyboardButton.WithUrl("Restart", deeplink);

        // Ignore if user is in rename state
        DeleteState<RenameState>(From);

        var knownUser = await memory.SeenUsers
            .SingleOrDefaultAsync(x => x.TelegramId == From.Id);

        if (knownUser is SeenUser seen)
            await Response($"Welcome back my friend {seen.Name}!", replyMarkup: button);
        else
        {
            memory.SeenUsers.Add(new()
            {
                TelegramId = From.Id,
                Name = From.FirstName,
            });

            await memory.SaveChangesAsync();

            await Response("Hello traveler!", replyMarkup: button);
        }
    }
}

[Command(deepLinkArg: "restart", joinArgs: true), Private]
internal class Restart() : MessageHandler
{
    protected override async Task HandleAsync(
        MessageContainer container,
        IServiceScope? scope = null,
        CancellationToken cancellationToken = default)
    {
        await Response("Restarted", cancellationToken: cancellationToken);
    }
}
