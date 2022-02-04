// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using TelegramUpdater;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers.SealedHandlers;

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
    await container.Response($"A job that takes 30 secs to be done");

    var response = await container.ChannelUserResponse();

    if (response != null)
    {
        await container.Response("You said " + response.Text!);
    }
    else
    {
        await container.Response("Timed Out!");
    }

    container.StopPropagation(); // Do not go any further
},
FilterCutify.OnCommand("start")));

updater.AddUpdateHandler(new MessageHandler(
    async container =>  await container.Response($"Next one!"),
    FilterCutify.OnCommand("start")));

await updater.Start();