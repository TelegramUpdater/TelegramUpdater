using Microsoft.Extensions.Logging;

namespace TelegramUpdater.Hosting;

internal class SimpleWriterService(IUpdater updater) : AbstractUpdateWriterService(updater)
{
    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (UpdaterOptions.FlushUpdatesQueue)
        {
            Logger.LogInformation("Flushing updates.");
            await BotClient.GetUpdates(-1, 1, 0, cancellationToken: stoppingToken).ConfigureAwait(false);
        }

        var offset = 0;
        var timeOut = 1000;

        Logger.LogInformation("Started Polling from writer service!");

        while (true)
        {
            try
            {
                var updates = await BotClient.GetUpdates(
                    offset,
                    100,
                    timeOut,
                    allowedUpdates: UpdaterOptions.AllowedUpdates,
                    cancellationToken: stoppingToken)
                .ConfigureAwait(false);

                foreach (var update in updates)
                {
                    await EnqueueUpdateAsync(update, stoppingToken).ConfigureAwait(false);
                    offset = update.Id + 1;
                }
            }
            catch (Exception e)
            {
                Logger.LogCritical(exception: e, "Auto update writer stopped due to an exception.");
                Updater.EmergencyCancel();
                break;
            }
        }
    }
}
