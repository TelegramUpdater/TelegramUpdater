// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
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

var myStartHandler = new MessageHandler(
    async container => await container.Response($"Next era!"),
    FilterCutify.OnCommand("start"));

updater.AddUpdateHandler(myStartHandler);

updater.AddScopedMessage<MyScopedMessageHandler>();
    // This type of handlers are Scoped!
    // In every request, a new instance of MyScopedMessageHandler is created.
    // These are useful in DI senarios or when you'r using any Scoped object
    // Like DbContexts.
    // Filters for such a handlers can applied using ApplyFilterAttribute

await updater.Start(); // 🔥 Fire up and block!