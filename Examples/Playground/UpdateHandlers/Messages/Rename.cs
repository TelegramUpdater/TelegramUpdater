// Ignore Spelling: cntr

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.Attributes;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("rename"), Private]
internal class Rename : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer cntr)
    {
        if (From is null) return;

        DeleteState<RenameState>(From);

        await Response(
            "Ok let's rename you! What is your name?",
            replyMarkup: new ForceReplyMarkup());

        InitiateState<RenameState>(From);
    }
}

[ScopedHandler(Group = 1)]
[Text, Renameing(RenameState.AskingName), Private]
internal class RenameAskName : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer cntr)
    {
        if (From is null) return;

        switch (ActualUpdate)
        {
            case { Text: { } text } when !string.IsNullOrWhiteSpace(text):                
                await cntr.Response(
                    $"Ok {text}, what is your last name?",
                    replyMarkup: new ForceReplyMarkup());

                Updater.MemoryCache.Set(From.Id, text, TimeSpan.FromSeconds(30));

                ForwardState<RenameState>(From);

                break;
                
            default:                
                await cntr.Response("Please provide a name.");
                break;
        }
    }
}

[ScopedHandler(Group = 2)]
[Text, Renameing(RenameState.AskingLastName), Private]
internal class RenameAskLastName(PlaygroundMemory memory) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer cntr)
    {
        if (From is null) return;

        switch (ActualUpdate)
        {
            case { Text: { } text } when !string.IsNullOrWhiteSpace(text):
                var fullName = $"{Updater[From.Id]} {text}";

                var updated = await memory.SeenUsers
                    .Where(x => x.TelegramId == From.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(x => x.Name, fullName));

                if (updated == 1)
                {
                    await cntr.Response(
                        $"Ok {text}, you are now renamed to {fullName}!",
                        replyMarkup: new ReplyKeyboardRemove());
                }
                else
                {
                    await Response("Did you called /start?");
                }

                Updater.MemoryCache.Remove(From.Id);
                DeleteState<RenameState>(From);

                break;
                
            default:                
                await cntr.Response("Please provide a last name.");
                break;
                
        }
    }
}


internal enum RenameState
{
    AskingName,
    AskingLastName,
}

internal class RenameingAttribute(RenameState state)
    : UserEnumStateAttribute<RenameState>(state);