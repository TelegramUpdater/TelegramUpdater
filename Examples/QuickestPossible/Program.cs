using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;

var updater = new Updater(
    new TelegramBotClient("BotToken"),
    new UpdaterOptions(allowedUpdates: new[] { UpdateType.Message, UpdateType.CallbackQuery }))

    .AddDefaultExceptionHandler()
    .AutoCollectScopedHandlers();

await updater.StartAsync();