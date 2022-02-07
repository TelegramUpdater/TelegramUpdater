// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.UpdateHandlers.SealedHandlers;
using UpdaterProduction;


// 1. ---------- Set things up ----------

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


// 2. ---------- Add Exception Handlers ----------

// All 'Exception's are handled. hmmm i'm not so sure
updater.AddExceptionHandler<Exception>(
    x =>
    {
        mainLogger.LogError(exception: x, "Exception in handler.");
        return Task.CompletedTask;
    });

// Only 'ApiRequestException's occured in 'MyScopedMessageHandler' will be handled.
updater.AddExceptionHandler<ApiRequestException, MyScopedMessageHandler>(
    x =>
    {
        mainLogger.LogWarning(exception: x, "Api Exception in handler.");
        return Task.CompletedTask;
    });


// 3. ---------- Add Update Handlers ----------

var myStartHandler = new MessageHandler(
    async container => await container.Response($"Are you ok? answer quick!"),
    FilterCutify.OnCommand("start"));

updater.AddUpdateHandler(myStartHandler);

// This type of handlers are Scoped!
// In every request, a new instance of MyScopedMessageHandler is created.
// These are useful in DI senarios or when you'r using any Scoped object
// Like DbContexts.
// Filters for such a handlers can applied using ApplyFilterAttribute
updater.AddScopedMessage<MyScopedMessageHandler>();

updater.AddScopedHandler<Message>(typeof(AboutMessageHandler)); // Other way
// Can be done with: updater.AddScopedMessage<AboutMessageHandler>();

// 4. ---------- Start! ----------

await updater.Start(true); // 🔥 Fire up and block!