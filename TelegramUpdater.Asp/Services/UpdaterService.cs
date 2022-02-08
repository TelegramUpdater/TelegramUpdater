using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramUpdater.Asp.Services
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updater.Logger.LogInformation("Executing updater start.");
            await _updater.Start(true, true, true, stoppingToken);
        }
    }
}
