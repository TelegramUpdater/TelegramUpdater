using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace QuickestPossible.UpdateHandlers.Messages
{
    [Command("ok")]
    internal sealed class StartCommandHandler : MessageHandler
    {
        protected override async Task HandleAsync(IContainer<Message> cntr)
        {
            var msg = await ResponseAsync("Are ya ok?",
                replyMarkup: MarkupExtensions.BuildInlineKeyboards(x =>
                    x.AddItem(InlineKeyboardButton.WithCallbackData("Yes"))
                    .AddItem(InlineKeyboardButton.WithCallbackData("No"))));

            var callback = await cntr.ChannelUserClick(TimeSpan.FromMinutes(30), new(@"Yes|No"));

            if (callback is not null)
            {
                await callback.EditAsync(text: $"Why {callback.Update.Data}?");
            }
            else
            {
                await ResponseAsync("Slow");
            }
        }
    }
}
