using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Controller.Attributes;
using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("about")]
internal class About : MessageControllerHandler
{
    [HandlerAction]
    [ChatType(ChatTypeFlags.Private)]
    public async Task Private([ResolveService] PlaygroundMemory memory)
    {
        var seenUsers = await memory.SeenUsers.CountAsync();
        await Response($"This's what you need to know about me: I've seen {seenUsers} people so far.");
    }

    [HandlerAction]
    [ChatType(ChatTypeFlags.Group | ChatTypeFlags.SuperGroup)]
    public async Task Group(IContainer<Message> container)
    {
        var aboutDeeplink = await Updater.CreateDeeplink("about");
        var aboutButton = InlineKeyboardButton.WithUrl("Continue in private", aboutDeeplink);
        await container.Response(
            "Theses are private talks!",
            replyMarkup: new InlineKeyboardMarkup(aboutButton));
    }
}

[Command(deepLinkArg: "about", joinArgs: true)]
internal class DeepAbout : MessageHandler
{
    protected override async Task HandleAsync()
    {
        await Response("This's what you need to know about me:");
    }
}
