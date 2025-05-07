// See https://aka.ms/new-console-template for more information

using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

var updater = new Updater("YOUR_BOT_TOKEN")
    .AddDefaultExceptionHandler()
    .AddMessageHandler(
        async (MessageContainer container) =>
        {
            await container.Response("Hello World");
        },
        ReadyFilters.OnCommand("start") & ReadyFilters.PM()
    );

await updater.Start();

