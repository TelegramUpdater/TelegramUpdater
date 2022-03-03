using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace QuickestPossible.UpdateHandlers.Messages
{
    [Command("ok")]
    internal sealed class StartCommandHandler : ScopedMessageHandler
    {
        protected override async Task HandleAsync(IContainer<Message> cntr)
        {
            var msg = await cntr.Response(
                "Are ya ok?",
                replyMarkup: MarkupExtensions.BuildInlineKeyboards(x =>
                    x.AddItem(InlineKeyboardButton.WithCallbackData("Yes"))
                    .AddItem(InlineKeyboardButton.WithCallbackData("No"))));

            var callback = await cntr.ChannelUserClick(TimeSpan.FromMinutes(30), @"Yes|No");

            if (callback is not null)
            {
                await callback.Edit(text: $"Why {callback.Update.Data}?");
            }
            else
            {
                await msg.Edit("Slow ...");
            }
        }
    }
}
