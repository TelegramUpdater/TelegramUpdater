namespace TelegramUpdater.RainbowUtilities
{
    /// <summary>
    /// Information about an owner of a queue.
    /// </summary>
    public readonly struct OwnerInfo<TId> where TId : struct
    {
        /// <summary>
        /// Create an instanse of ownerinfo
        /// </summary>
        internal OwnerInfo(TId ownerId, ushort queueId, DateTime queueStart)
        {
            OwnerId = ownerId;
            QueueId = queueId;
            QueueStart = queueStart;
        }

        /// <summary>
        /// Owner id.
        /// </summary>
        public TId OwnerId { get; }

        /// <summary>
        /// The queue assigned for the owner
        /// </summary>
        public ushort QueueId { get; }

        /// <summary>
        /// Start of the queue
        /// </summary>
        public DateTime QueueStart { get; }
    }
}
