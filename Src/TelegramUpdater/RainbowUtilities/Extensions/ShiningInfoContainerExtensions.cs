using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// A set of extension methods for shining info.
/// </summary>
public static class ShiningInfoContainerExtensions
{
    /// <summary>
    /// Tries to drop all pending objects for a given queue.
    /// </summary>
    public static bool DropPendingAsync<T>(this IContainer<T> container) where T : class
        => container.ShiningInfo.DropPendingAsync();

    /// <summary>
    /// Tries to count pending objects of the queue.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="count">Returned count of the queue.</param>
    /// <returns></returns>
    public static bool TryCountPending<T>(this IContainer<T> container, out int? count) where T : class
        => container.ShiningInfo.TryCountPending(out count);

    /// <summary>
    /// Tries to get next item in the queue.
    /// </summary>
    /// <param name="timeOut">Returns default on this timeout!</param>
    /// <param name="container">The container.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    public static async ValueTask<ShiningInfo<long, Update>?> ReadNextAsync<T>(
        this IContainer<T> container,
        TimeSpan timeOut,
        CancellationToken cancellationToken = default) where T : class
        => await container.ReadNextAsync(timeOut, cancellationToken).ConfigureAwait(false);
}
