using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramUpdater.Hosting;

internal class SimpleWriterService : UpdateWriterServiceAbs
{
    public SimpleWriterService(IUpdater updater) : base(updater)
    {
    }

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (UpdaterOptions.FlushUpdatesQueue)
        {
            Logger.LogInformation("Flushing updates.");
            await BotClient.GetUpdatesAsync(-1, 1, 0, cancellationToken: stoppingToken);
        }

        var offset = 0;
        var timeOut = 1000;

        Logger.LogInformation("Started Polling from writer service!");

        while (true)
        {
            try
            {
                var updates = await BotClient.GetUpdatesAsync(offset,
                                                              100,
                                                              timeOut,
                                                              allowedUpdates: UpdaterOptions.AllowedUpdates,
                                                              cancellationToken: stoppingToken);
                foreach (var update in updates)
                {
                    await EnqueueUpdateAsync(update, stoppingToken);
                    offset = update.Id + 1;
                }
            }
            catch (Exception e)
            {
                Logger.LogCritical(exception: e, "Auto update writer stopped due to an ecxeption.");
                Updater.EmergencyCancel();
                break;
            }
        }
    }
}
