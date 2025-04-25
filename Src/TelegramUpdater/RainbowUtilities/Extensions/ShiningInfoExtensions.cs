using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TelegramUpdater.RainbowUtilities
{
    /// <summary>
    /// A set of extension methods for shining info.
    /// </summary>
    public static class ShiningInfoExtensions
    {
        /// <summary>
        /// Tries to drop all pending objects for a given queue.
        /// </summary>
        public static bool DropPendingAsync<TId, TValue>(
            this ShiningInfo<TId, TValue> shiningInfo) where TId : struct
            => shiningInfo.Rainbow.DropPendingAsync(shiningInfo.ProcessId);

        /// <summary>
        /// Tries to count pending objects of the queue.
        /// </summary>
        /// <param name="count">Returned count of the queue.</param>
        /// <param name="shiningInfo">The shining info.</param>
        /// <returns></returns>
        public static bool TryCountPending<TId, TValue>(
            this ShiningInfo<TId, TValue> shiningInfo,
            [NotNullWhen(true)] out int? count) where TId : struct
            => shiningInfo.Rainbow.TryCountPending(
                shiningInfo.ProcessId, out count);

        /// <summary>
        /// Tries to get next item in the queue.
        /// </summary>
        /// <param name="shiningInfo">The shining info.</param>
        /// <param name="timeOut">Returns default on this timeout!</param>
        /// <param name="cancellationToken">To cancel the job.</param>
        public static async ValueTask<ShiningInfo<TId, TValue>?> ReadNextAsync<TId, TValue>(
            this ShiningInfo<TId, TValue> shiningInfo,
            TimeSpan timeOut,
            CancellationToken cancellationToken = default)
            where TId : struct
            => await shiningInfo.Rainbow.ReadNextAsync(
                shiningInfo.ProcessId, timeOut, cancellationToken);

        /// <summary>
        /// Yields all <typeparamref name="TValue"/>s from given queueId.
        /// </summary>
        /// <param name="cancellationToken">Cancel the job.</param>
        /// <param name="shiningInfo">The shining info.</param>
        public static async IAsyncEnumerable<ShiningInfo<TId, TValue>> YieldAsync<TId, TValue>(
            this ShiningInfo<TId, TValue> shiningInfo,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
            where TId : struct
        {
            await foreach (var item in shiningInfo.Rainbow.YieldAsync(
                shiningInfo.ProcessId, cancellationToken))
            {
                yield return item;
            }
        }
    }
}
