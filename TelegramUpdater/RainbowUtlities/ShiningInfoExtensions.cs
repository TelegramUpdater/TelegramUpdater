using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramUpdater.RainbowUtlities
{
    public static class ShiningInfoExtensions
    {
        /// <summary>
        /// Tries to drop all pending objects for a given queue.
        /// </summary>
        public static bool DropPendingAsync<TId, TValue>(
            this ShiningInfo<TId, TValue> shinigInfo) where TId : struct
            => shinigInfo.Rainbow.DropPendingAsync(shinigInfo.ProcessId);

        /// <summary>
        /// Tries to count pending objects of the queue.
        /// </summary>
        /// <param name="count">Returned count of the queue.</param>
        /// <returns></returns>
        public static bool TryCountPending<TId, TValue>(
            this ShiningInfo<TId, TValue> shinigInfo, [NotNullWhen(true)] out int? count) where TId : struct
            => shinigInfo.Rainbow.TryCountPending(shinigInfo.ProcessId, out count);

        /// <summary>
        /// Tries to get next item in the queue.
        /// </summary>
        /// <param name="timeOut">Returns default on this timeout!</param>
        public static async ValueTask<ShiningInfo<TId, TValue>?> ReadNextAsync<TId, TValue>(
            this ShiningInfo<TId, TValue> shinigInfo,
            TimeSpan timeOut,
            CancellationToken cancellationToken = default)
            where TId : struct
            => await shinigInfo.Rainbow.ReadNextAsync(
                shinigInfo.ProcessId, timeOut, cancellationToken);
    }
}
