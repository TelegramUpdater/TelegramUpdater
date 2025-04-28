using Microsoft.Extensions.Hosting;

namespace TelegramUpdater.Hosting;

/// <summary>
/// Use this abstract class to build your custom update writer as a background service.
/// </summary>
/// <remarks>
/// <see cref="Updater"/> Should exists in <see cref="IServiceProvider"/>
/// </remarks>
/// <inheritdoc/>
public class UpdateWriterService<TWriter>(TWriter writer)
    : IHostedService, IDisposable where TWriter : AbstractUpdateWriter
{
    private Task? _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new();

    /// <summary>
    /// The <see cref="AbstractUpdateWriter"/>.
    /// </summary>
    public TWriter Writer { get; } = writer;

    /// <inheritdoc/>
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        // Store the task we're executing
        _executingTask = Writer.ExecuteAsync(_stoppingCts.Token);

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
#if NET8_0_OR_GREATER
            await _stoppingCts.CancelAsync().ConfigureAwait(false);
#else
            _stoppingCts.Cancel();
#endif
        }
        finally
        {
            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                cancellationToken)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _stoppingCts.Cancel();
    }
}
