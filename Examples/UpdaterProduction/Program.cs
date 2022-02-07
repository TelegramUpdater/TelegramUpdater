// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.ExceptionHandlers;
using UpdaterProduction;


var updater = new UpdaterBuilder(
    "1506599454:AAHnsr-UiF8nb2xUlux1EI53dMBUxhFAF0A")
    .StepOne(
        maxDegreeOfParallelism: 10,   // maximum update process tasks count at the same time
                                      // Eg: first 10 updates are answers quickly, but others should wait
                                      // for any of that 10 to be done.

        perUserOneByOneProcess: true, // a user should finish a request to go to next one.
        allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery })

    .StepTwo(inherit: false)

    .StepTwo(CommonExceptions.ParsingException(
        (updater, ex) =>
        {
            updater.Logger.LogWarning(exception: ex, "Handler has entity parsing error!");
            return Task.CompletedTask;
        },
        allowedHandlers: new[]
        {
            typeof(AboutMessageHandler),
            typeof(MyScopedMessageHandler)
        }))

    .StepThree(
        async container => await container.Response("Started!"),
        FilterCutify.OnCommand("start"))

    .AddScopedMessage<MyScopedMessageHandler>()
    
    .AddScopedHandler<Message>(typeof(AboutMessageHandler)); // Other way
                                                             // Can be done with: updater.AddScopedMessage<AboutMessageHandler>();


// ---------- Start! ----------

var me = await updater.GetMeAsync();
updater.Logger.LogInformation("Start listening to {username}", me.Username);

await updater.Start(true); // 🔥 Fire up and block!