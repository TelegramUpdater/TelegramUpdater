using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater
{
    /// <summary>
    /// Sets options for <see cref="IUpdater"/>.
    /// </summary>
    public readonly struct UpdaterOptions
    {
        /// <summary>
        /// Sets options for <see cref="IUpdater"/>.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">Maximum number of allowed concurent update handling tasks.</param>
        /// <param name="perUserOneByOneProcess">User should wait for a request to finish to start a new one.</param>
        /// <param name="logger">If you want to use your own logger.</param>
        /// <param name="cancellationToken">Default token to be used in Start method.</param>
        /// <param name="flushUpdatesQueue">Old updates will gone.</param>
        /// <param name="allowedUpdates">Allowed updates.</param>
        public UpdaterOptions(
            int? maxDegreeOfParallelism = default,
            bool perUserOneByOneProcess = true,
            ILogger<IUpdater>? logger = default,
            CancellationToken cancellationToken = default,
            bool flushUpdatesQueue = false,
            UpdateType[]? allowedUpdates = default)
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            PerUserOneByOneProcess = perUserOneByOneProcess;
            Logger = logger;
            CancellationToken = cancellationToken;
            FlushUpdatesQueue = flushUpdatesQueue;
            AllowedUpdates = allowedUpdates ?? Array.Empty<UpdateType>();
        }

        /// <summary>
        /// Maximum number of allowed concurent update handling tasks.
        /// </summary>
        public int? MaxDegreeOfParallelism { get; }

        /// <summary>
        /// User should wait for a request to finish to start a new one.
        /// </summary>
        public bool PerUserOneByOneProcess { get; }

        /// <summary>
        /// If you want to use your own logger.
        /// </summary>
        public ILogger<IUpdater>? Logger { get; }

        /// <summary>
        /// Default token to be used in Start method.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Old updates will gone.
        /// </summary>
        public bool FlushUpdatesQueue { get; }

        /// <summary>
        /// Allowed updates.
        /// </summary>
        public UpdateType[] AllowedUpdates { get; }
    }
}
