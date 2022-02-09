using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.Hosting;

namespace ManualWriterWorker
{
    /// <summary>
    /// This manual wrtiter uses <see href="https://github.com/TelegramBots/Telegram.Bot.Extensions.Polling"/>
    /// Package to receive updates and write them to updater.
    /// </summary>
    public class MyManualWriter : UpdateWriterServiceAbs
    {
        public MyManualWriter(Updater updater) : base(updater)
        {
        }

        protected override async Task GetUpdatesProcess(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            try
            {
                await Updater.BotClient.ReceiveAsync(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    Updater.EmergencyToken
                );
            }
            catch (OperationCanceledException)
            {
            }
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            await Write(update, cancellationToken);
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                Updater.Logger.LogError(exception: apiRequestException, message: "Error in MyManualWriter");
            }
            return Task.CompletedTask;
        }

    }
}