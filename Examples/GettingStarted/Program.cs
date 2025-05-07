// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
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
    )
    .AddCallbackQueryHandler(
        async (CallbackQueryContainer container) =>
        {
            await container.Edit("Thank you for answering");
            await container.Answer();
        },
        ReadyFilters.DataMatches("(yes|no)", RegexOptions.IgnoreCase)
    );

await updater.Start();

