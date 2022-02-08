using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.Hosting;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTelegramUpdater(
            "2015323878:AAEBfa-pTNt4fC9O1_Gw3FD9ZnreySiWhc8",
            new UpdaterOptions(
                maxDegreeOfParallelism: 10, // maximum update process tasks count at the same time
                                            // Eg: first 10 updates are answers quickly, but others should wait
                                            // for any of that 10 to be done.

                perUserOneByOneProcess: true, // a user should finish a request to go to next one.
                allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery }),

            (builder) =>
                builder.AddMessageHandler<SimpleMessageHandler>());
    })
    .Build();

await host.RunAsync();
