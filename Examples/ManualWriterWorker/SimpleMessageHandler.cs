using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace ManualWriterWorker
{
    [ApplyFilter(typeof(PrivateHelloCommand))]
    public class SimpleMessageHandler : ScopedMessageHandler
    {
        protected override async Task HandleAsync(UpdateContainerAbs<Message> container)
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
                    await msg.Edit("Slow");
                });
        }
    }

    internal class PrivateHelloCommand : Filter<Message>
    {
        public PrivateHelloCommand()
            : base(FilterCutify.OnCommand("hello") & FilterCutify.PM())
        {
        }
    }
}
