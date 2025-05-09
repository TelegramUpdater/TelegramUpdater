using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;

namespace TelegramUpdater.UpdateWriters;

internal class DefaultUpdateWriter : AbstractUpdateWriter
{
    public DefaultUpdateWriter()
    {

    }

    public DefaultUpdateWriter(IUpdater? updater = default)
    {
        if (updater is not null)
            Updater = updater;
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await EnqueueUpdate(update, cancellationToken).ConfigureAwait(false);
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Logger.LogError(
            message: "Error while getting updates.", exception: exception);
        return Task.CompletedTask;
    }

    protected override async Task Execute(CancellationToken stoppingToken)
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
            // Ignored
        }
    }
}
