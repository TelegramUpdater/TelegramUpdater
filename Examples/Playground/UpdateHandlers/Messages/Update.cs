using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateHandlers.Controller.Attributes;
using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("update"), Private]
internal class Update : MessageControllerHandler
{
    [HandlerAction(IgnoreIfParametersNotFound = true)] // Default
    public async Task UpdateName(
        // By default command arguments are Required and they will be searched for in controller's filter.
        // If the required argument can't be found, the action will be ignored,
        // since `IgnoreIfParametersNotFound = true` by default.
        [CommandArg(0,
            Required = true, // Default
            IncludeController = true // Default
        )] long userId,
        [CommandArg(1, JoinToEnd = true)] string name,
        [ResolveService] PlaygroundMemory memory)
    {
        var updated = await memory.SeenUsers
            .Where(x => x.TelegramId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Name, name));

        if (updated >= 1)
            await Response($"User [{userId}] renamed to '{name}'");
        else
        {
            var nameCode = string.Join('-', name.Split(' '));
            await Response($"No such user with id [{userId}] is seen!, Wanna insert one?",
                replyMarkup: new InlineKeyboardMarkup([[
                    InlineKeyboardButton.WithCallbackData("Yes", $"insertSeen_{userId}_{nameCode}"),
                    InlineKeyboardButton.WithCallbackData("No", $"insertSeen")]]));
        }
    }

    [HandlerAction]
    public async Task InvalidArguments()
    {
        await Response($"Provided arguments are incorrect!\n- Help: /update 12345 (number) name (string)");
    }
}
