using Microsoft.Extensions.Logging;

namespace TelegramUpdater;

/// <summary>
/// Use this class to create an update writer ( to the updater ).
/// </summary>
/// <remarks>
/// If you're not using an <see cref="IServiceProvider"/>, the subclass
/// you're building Should have parameterless constructor.
/// <para>
/// In case of <see cref="IServiceProvider"/>, This type and parameters
/// should exists in service collection.
/// </para>
/// </remarks>
public abstract class AbstractUpdateWriter
{
    /// <summary>
    /// Create a default instance of update writer.
    /// </summary>
    protected AbstractUpdateWriter() { }

    /// <summary>
    /// Create a default instance of update writer.
    /// </summary>
    /// <param name="updater">The updater.</param>
    protected AbstractUpdateWriter(IUpdater updater)
    {
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));
    }

    internal void SetUpdater(IUpdater updater)
    {
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));
    }

    /// <summary>
    /// Jobs you wanna do before running the <see cref="Execute(CancellationToken)"/>.
    /// </summary>
    /// <remarks>
    /// By default, it just detects <see cref="UpdaterOptions.AllowedUpdates"/> automatically if it's null using
    /// <see cref="IUpdater.DetectAllowedUpdates"/>.
    /// </remarks>
    /// <returns></returns>
    protected virtual Task BeforeExecution(CancellationToken stoppingToken)
    {
        if (UpdaterOptions.AllowedUpdates == null)
        {
            UpdaterOptions.AllowedUpdates = Updater.DetectAllowedUpdates();

            Logger.LogInformation(
                "Detected allowed updates automatically {allowed}",
                string.Join(", ", UpdaterOptions.AllowedUpdates.Select(x => x.ToString()))
            );
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Implement your stuff to get updates and write them to the updater
    /// Using <see cref="EnqueueUpdate(Update, CancellationToken)"/>.
    /// </summary>
    /// <param name="stoppingToken">Cancel the job.</param>
    /// <returns></returns>
    protected abstract Task Execute(CancellationToken stoppingToken);

    /// <summary>
    /// Start running the writer.
    /// </summary>
    /// <param name="stoppingToken">Cancel the job.</param>
    /// <returns></returns>
    public async Task Run(CancellationToken stoppingToken)
    {
        await BeforeExecution(stoppingToken).ConfigureAwait(false);
        await Execute(stoppingToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updater instance.
    /// </summary>
    protected IUpdater Updater { get; set; } = null!;

    /// <summary>
    /// Telegram bot client instance.
    /// </summary>
    protected ITelegramBotClient BotClient => Updater.BotClient;

    /// <summary>
    /// Logger instance of this <see cref="Updater"/>
    /// </summary>
    protected ILogger<IUpdater> Logger => Updater.Logger;

    /// <summary>
    /// <see cref="Updater"/>'s options.
    /// </summary>
    protected UpdaterOptions UpdaterOptions => Updater.UpdaterOptions;

    /// <summary>
    /// Use this to add your update at the end of processing queue.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected async ValueTask EnqueueUpdate(
        Update update, CancellationToken cancellationToken)
    {
        await Updater.Write(update, cancellationToken).ConfigureAwait(false);
    }

    internal static AbstractUpdateWriter Create<TWriter>(IUpdater updater)
        where TWriter : AbstractUpdateWriter
    {
        var writer = (AbstractUpdateWriter?)Activator.CreateInstance(
            typeof(TWriter), [updater]);

        return writer ?? throw new InvalidOperationException(
                $"Can't create {typeof(TWriter)}");
    }
}
