// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton.Attributes;

var updater = new Updater(
    new UpdaterOptions(
        botToken: ReplaceWith.YourOwn.BOT_TOKEN, // Your bot token from @BotFather.

        maxDegreeOfParallelism: 10, // maximum update process tasks count at the same time
                                    // Eg: first 10 updates are answers quickly, but others should wait
                                    // for any of that 10 to be done.

        flushUpdatesQueue: true, // If you don't care about missing update when you're asleep.
        allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]
));

updater
    .AddDefaultExceptionHandler() // A console logger
    .QuickStartCommandReply("Hello there!")
    .CollectSingletonHandlers(); // Collect methods marked by SingletonHandlerCallback.

// ---------- Start! ----------

var me = await updater.GetMe();
updater.Logger.LogInformation("Start listening to {username}", me.Username);

await updater.Start(); // 🔥 Fire up and block!

partial class Program
{
    // Message handler with command filter
    [Command("rate"), ChatType(ChatTypeFlags.Private)]
    [SingletonHandlerCallback(UpdateType.Message)]
    public static async Task RateHandler(IContainer<Message> container)
    {
        var markup = MarkupExtensions.BuildInlineKeyboards(builder => builder
            .AddCallbackQuery("Very much", "rating_5")
            .AddCallbackQuery("I like you", "rating_4")
            .AddCallbackQuery("I don't know", "rating_3")
            .AddCallbackQuery("Not likely", "rating_2")
            .AddCallbackQuery("Hate you", "rating_1"),
            rowCapacity: 2);

        await container.Response("How much do you like us?",
            replyMarkup: markup);
    }

    // Callback query handler with regex filter on CallbackQuery.Data.
    [Regex(@"^rating_(?<rate>\d{1})$")]
    [SingletonHandlerCallback(UpdateType.CallbackQuery)]
    public static async Task RateAnswerHandler(IContainer<CallbackQuery> container)
    {
        if (container.TryGetMatchCollection(out var matches))
        {
            var rate = matches[0].Groups["rate"].Value;
            await container.Edit($"Thank you! Your rating was {rate}.");
            await container.Answer();
        }
    }
}