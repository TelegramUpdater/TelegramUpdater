using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Webhook.UpdateHandlers.Messages;

[Command("rate"), ChatType(ChatTypeFlags.Private)]
internal class Rate : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        var markup = MarkupExtensions.BuildInlineKeyboards(builder => builder
            .AddCallbackQuery("Very much", "rating_5")
            .AddCallbackQuery("I like you", "rating_4")
            .AddCallbackQuery("I don't know", "rating_3")
            .AddCallbackQuery("Not likely", "rating_2")
            .AddCallbackQuery("Hate you", "rating_1"),
            rowCapacity: 2);

        await Response("How much do you like us?", replyMarkup: markup);
    }
}
