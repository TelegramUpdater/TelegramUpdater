using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramUpdater.Hosting
{
    public class UpdaterService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly Updater _updater;

        public UpdaterService(IServiceProvider services)
        {
            _services = services;

            using var scope = _services.CreateScope();
            _updater = scope.ServiceProvider.GetRequiredService<Updater>();
        }

        /// <summary>
        /// Indicates if the service should or should not write updates automatically
        /// </summary>
        /// <remarks>
        /// In webhook apps this is possibly true.
        /// </remarks>
        protected virtual bool ManualWriting => false;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updater.Logger.LogInformation("Executing updater start.");
            await _updater.Start(true, ManualWriting, true, stoppingToken);
        }
    }
}
