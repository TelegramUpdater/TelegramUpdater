using TelegramUpdater.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// this will collect updater options like BotToken, AllowedUpdates and ...
// from configuration section "TelegramUpdater". in this example from appsettings.json
builder.AddTelegramUpdater(
    (builder) => builder
        .QuickStartCommandReply("Hello there!")
        // Collect scoped handlers located for example at UpdateHandlers/Messages for messages.
        .CollectScopedHandlers()
        .AddDefaultExceptionHandler());

var host = builder.Build();
host.Run();
