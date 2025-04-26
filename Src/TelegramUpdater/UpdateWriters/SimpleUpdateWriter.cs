using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;

namespace TelegramUpdater.UpdateWriters;

internal class SimpleUpdateWriter : AbstractUpdateWriter
{
    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await EnqueueUpdateAsync(update, cancellationToken).ConfigureAwait(false);
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Logger.LogError(
            message: "Error while getting updates.", exception: exception);
        return Task.CompletedTask;
    }

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            DropPendingUpdates = UpdaterOptions.FlushUpdatesQueue,
            AllowedUpdates = UpdaterOptions.AllowedUpdates, // receive all update types
        };

        try
        {
            await BotClient.ReceiveAsync(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                stoppingToken
            ).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }
}
