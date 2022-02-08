using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramUpdater.Asp.Services
{
    public class UpdaterService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly Updater _updater;
        private readonly CancellationTokenSource _cts;

        public UpdaterService(IServiceProvider services)
        {
            _services = services;

            using var scope = _services.CreateScope();
            _updater = scope.ServiceProvider.GetRequiredService<Updater>();
            _cts = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _updater.Start(false, true, true, _cts.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
