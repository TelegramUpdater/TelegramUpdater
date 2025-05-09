using Microsoft.EntityFrameworkCore;

namespace Playground;

internal class UpgradeMemory(
    IServiceProvider serviceProvider,
    ILogger<UpgradeMemory> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var memory = scope.ServiceProvider.GetRequiredService<PlaygroundMemory>();

        var pending = await memory.Database.GetPendingMigrationsAsync(cancellationToken);

        if (pending.Any())
        {
            logger.LogInformation("Applying migrations: {migrations}", string.Join(", ", pending));
            await memory.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            logger.LogInformation($"Memory is up to date.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
