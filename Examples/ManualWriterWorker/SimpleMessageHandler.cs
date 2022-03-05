using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace ManualWriterWorker;

[Command("ok"), ChatType(ChatTypeFlags.Private)]
public class SimpleMessageHandler : ScopedMessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> container)
    {
        var msg = await container.Response($"Are you ok? answer quick!",
            replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Yes i'm OK!", "ok")));

        await container.ChannelUserResponse(TimeSpan.FromMinutes(5))
            .IfNotNull(async answer =>
            {
                await answer.Response("Well ...");
            })
            .Else(async _ =>
            {
                await container.Response("Slow", sendAsReply: false);
            });
    }
}
