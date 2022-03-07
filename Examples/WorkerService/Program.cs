using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.Hosting;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTelegramUpdater(
            "BOT_TOKEN",
            new UpdaterOptions(
                maxDegreeOfParallelism: 10, // maximum update process tasks count at the same time
                                            // Eg: first 10 updates are answers quickly, but others should wait
                                            // for any of that 10 to be done.
                allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery }),

            (builder) => builder
                .AddScopedUpdateHandler<SimpleMessageHandler, Message>()
                .AddDefaultExceptionHandler());
    })
    .Build();

await host.RunAsync();
