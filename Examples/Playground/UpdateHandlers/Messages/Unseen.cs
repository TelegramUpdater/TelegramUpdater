using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("unseen"), Private]
internal class Unseen(PlaygroundMemory memory) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        if (From is null) return;

        var message = await Response(
            text: "Are you sure about becoming unseen?",
            replyMarkup: new InlineKeyboardMarkup([[
                InlineKeyboardButton.WithCallbackData("Yes"),
                InlineKeyboardButton.WithCallbackData("No")]]));

        // Wait for short coming answer right here
        var answer = await ChannelButtonClick(
            TimeSpan.FromSeconds(5),
            new(@"Yes|No"));

        switch (answer)
        {
            case { Update.Data: { } data }:
                {
                    // User did answer
                    if (data == "Yes")
                    {
                        await memory.SeenUsers
                            .Where(x => x.TelegramId == From.Id)
                            .ExecuteDeleteAsync();
                        await answer.Edit("Ok, you've become unseen.");
                    }
                    else
                        await answer.Edit("Yeah being unseen is hard.");

                    await answer.Answer();

                    break;
                }
            default:
                {
                    // Likely timed out.
                    await message.Edit("You're not.");
                    break;
                }
        }
    }
}
