namespace TelegramUpdater.RainbowUtlities
{
    /// <summary>
    /// Provides information about a process in <see cref="Rainbow{TId, TValue}"/>
    /// </summary>
    /// <typeparam name="TId">The object's id.</typeparam>
    /// <typeparam name="TValue">Object's type</typeparam>
    public sealed class ProcessorInfo<TId, TValue> where TId : struct
    {
        internal ProcessorInfo(ushort id, OwnerInfo<TId>? ownerId, TaskStatus? taskStatus, int pendingCount)
        {
            Id = id;
            OwnerId = ownerId;
            TaskStatus = taskStatus;
            PendingCount = pendingCount;
        }

        /// <summary>
        /// Process/Queue id.
        /// </summary>
        public ushort Id { get; }

        /// <summary>
        /// Object's owner inforamtion.
        /// </summary>
        public OwnerInfo<TId>? OwnerId { get; }

        /// <summary>
        /// Status of a task that acts for this queue.
        /// </summary>
        public TaskStatus? TaskStatus { get; }

        /// <summary>
        /// Pending objects count in queue.
        /// </summary>
        public int PendingCount { get; }
    }
}
