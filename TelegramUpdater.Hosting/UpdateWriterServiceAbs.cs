using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramUpdater.Hosting
{
    /// <summary>
    /// Use this abstract class to build your custome update writer as a background service.
    /// </summary>
    /// <remarks>
    /// <see cref="Updater"/> Should exsits in <see cref="IServiceProvider"/>
    /// </remarks>
    public abstract class UpdateWriterServiceAbs: BackgroundService
    {
        private readonly Updater _updater;

        /// <summary>
        /// Use this abstract class to build your custome update writer as a background service.
        /// </summary>
        /// <remarks>
        /// <see cref="Updater"/> Should exsits in <see cref="IServiceProvider"/>
        /// </remarks>
        protected UpdateWriterServiceAbs(Updater updater)
        {
            _updater = updater;
        }

        /// <summary>
        /// Here you should implement code to getUpdates.
        /// <para>
        /// After getting the update, call <see cref="Write(Update)"/> to write it to the updater.
        /// </para>
        /// </summary>
        /// <returns></returns>
        protected abstract Task GetUpdatesProcess(CancellationToken stoppingToken);

        /// <summary>
        /// Write update.
        /// </summary>
        protected async Task Write(Update update)
        {
            await _updater.WriteUpdateAsync(update);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await GetUpdatesProcess(stoppingToken).ConfigureAwait(false);
        }
    }
}
