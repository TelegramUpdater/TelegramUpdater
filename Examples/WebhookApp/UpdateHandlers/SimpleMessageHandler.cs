using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace WebhookApp.UpdateHandlers;

[Command("hello"), Private]
public class SimpleMessageHandler : ScopedMessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> container)
    {
        var msg = await container.Response($"Are you ok? answer quick!",
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Yes i'm OK!", "ok")));

        await container.ChannelUserClick(TimeSpan.FromSeconds(5), "ok")
            .IfNotNull(async answer =>
            {
                await answer.Edit(text: "Well ...");
            })
            .Else(async _ =>
            {
                await container.Response("Slow", sendAsReply: false);
            });
    }
}
