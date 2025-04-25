using System.Runtime.InteropServices;

namespace TelegramUpdater.RainbowUtilities;

/// <summary>
/// Information about an owner of a queue.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct OwnerInfo<TId> where TId : struct
{
    /// <summary>
    /// Create an instance of <see cref="OwnerInfo{TId}"/>
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
