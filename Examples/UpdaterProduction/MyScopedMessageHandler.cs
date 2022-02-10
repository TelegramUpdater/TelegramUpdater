using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace UpdaterProduction
{
    [ApplyFilter(typeof(PrivateTestCommand))]
    internal class MyScopedMessageHandler : ScopedMessageHandler
    {
        public MyScopedMessageHandler() : base(group: -1)
        { }

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
                    await msg.Edit("Slow");
                });
        }
    }

    internal class PrivateTestCommand : Filter<Message>
    {
        public PrivateTestCommand()
            : base(FilterCutify.OnCommand("test") & FilterCutify.PM())
        {
        }
    }
}
