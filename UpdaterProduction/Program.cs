// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers.SealedHandlers;
using UpdaterProduction;

// Do logging stuff.
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

var updater = new Updater(
    new TelegramBotClient("1506599454:AAHnsr-UiF8nb2xUlux1EI53dMBUxhFAF0A"),
    new UpdaterOptions(
        maxDegreeOfParallelism: 10,   // maximum update process tasks count at the same time
                                      // Eg: first 10 updates are answers quickly, but others should wait
                                      // for any of that 10 to be done.

        perUserOneByOneProcess: true, // a user should finish a request to go to next one.
        allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery },
        logger: loggerFactory.CreateLogger<Updater>()
    )
);

var mainLogger = loggerFactory.CreateLogger<Program>();
loggerFactory.Dispose(); // not required anymore

updater.AddExceptionHandler(new ExceptionHandler<Exception>(
    x =>
    {
        mainLogger.LogError(exception: x, "Exception in handler.");
        return Task.CompletedTask;
    }));

updater.AddExceptionHandler(new ExceptionHandler<ApiRequestException>(
    x =>
    {
        mainLogger.LogWarning(exception: x, "Api Exception in handler.");
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

await updater.Start(true); // 🔥 Fire up and block!