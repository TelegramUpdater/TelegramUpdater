using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramUpdater.Hosting;

/// <summary>
/// Use this abstract class to build your custome update writer as a background service.
/// </summary>
/// <remarks>
/// <see cref="Updater"/> Should exsits in <see cref="IServiceProvider"/>
/// </remarks>
public abstract class UpdateWriterServiceAbs : UpdateWriterAbs,
    IHostedService, IDisposable
{
    private Task? _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new();

    /// <inheritdoc/>
    protected UpdateWriterServiceAbs(IUpdater updater) : base(updater)
    {
    }

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        // Store the task we're executing
        _executingTask = ExecuteAsync(_stoppingCts.Token);

        // If the task is completed then return it,
        // this will bubble cancellation and failure to the caller
        if (_executingTask.IsCompleted)
        {
            return _executingTask;
        }

        // Otherwise it's running
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            _stoppingCts.Cancel();
        }
        finally
        {
            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                          cancellationToken));
        }
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _stoppingCts.Cancel();
    }
}
