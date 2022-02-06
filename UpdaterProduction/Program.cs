// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers.SealedHandlers;
using UpdaterProduction;

Console.WriteLine("Hello, World!");

var updater = new Updater(
    new TelegramBotClient("1506599454:AAHnsr-UiF8nb2xUlux1EI53dMBUxhFAF0A"),
    maxDegreeOfParallelism: 10,  // maximum update process tasks count at the same time
                                 // Eg: first 10 updates are answers quickly, but others should wait
                                 // for any of that 10 to be done.

    perUserOneByOneProcess: true // a user should finish a request to go to next one.
);

updater.AddExceptionHandler(new ExceptionHandler<Exception>(
    x =>
    {
        global::System.Console.WriteLine(x.Message);
        return Task.CompletedTask;
    }));

updater.AddUpdateHandler(new MessageHandler(async container =>
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
},
FilterCutify.OnCommand("start")));


var myStartHandler = new MessageHandler(
    async container => await container.Response($"Next one!"),
    FilterCutify.OnCommand("start"));

updater.AddUpdateHandler(myStartHandler);

updater.AddScopedHandler<MyScopedMessageHandler, Message>(
    FilterCutify.OnCommand("scope")); // This type of handlers are Scoped!
                                      // In every request, a new instance of MyScopedMessageHandler is created.
                                      // These are useful in DI senarios or when you'r using any Scoped object
                                      // Like DbContexts.

await updater.Start();