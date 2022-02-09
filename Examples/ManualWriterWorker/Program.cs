using ManualWriterWorker;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.Hosting;
using TelegramUpdater;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTelegramManualUpdater<MyManualWriter>( // Use your own manual update writer!
            "2015323878:AAEBfa-pTNt4fC9O1_Gw3FD9ZnreySiWhc8",
            new UpdaterOptions(
                maxDegreeOfParallelism: 10, // maximum update process tasks count at the same time
                                            // Eg: first 10 updates are answers quickly, but others should wait
                                            // for any of that 10 to be done.

                perUserOneByOneProcess: true, // a user should finish a request to go to next one.
                allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery }),

            (builder) => builder
                .AddMessageHandler<SimpleMessageHandler>()
                .AddExceptionHandler<Exception>(
                    (u, e) =>
                    {
                        u.Logger.LogWarning(exception: e, message: "Error while handlig ...");
                        return Task.CompletedTask;
                    }, inherit: true)
                );
    })
    .Build();

await host.RunAsync();