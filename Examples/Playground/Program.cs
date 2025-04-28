using Playground.UpdateHandlers.Messages;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.Hosting;
using TelegramUpdater.UpdateContainer;

var builder = Host.CreateApplicationBuilder(args);

var botToken = builder.Configuration.GetSection("TelegramUpdater:BotToken")
    .Get<string>() ?? throw new InvalidOperationException("Bot token not found.");

builder.Services.AddTelegramUpdater(
    botToken,
    new UpdaterOptions(
        allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]),

    (builder) => builder
        .AddMessageHandler<Start>()
        .Execute(updater => updater
            .AddSingletonUpdateHandler(
                UpdateType.Message,
                async (container) =>
                {
                    await container.ResponseAsync("Want me to help you?!");
                },
                FilterCutify.OnCommand("help")))
        .AddDefaultExceptionHandler());

var host = builder.Build();
await host.RunAsync();
