using System.Threading.Tasks;

namespace TelegramUpdater.TrafficLights
{
    public readonly struct CrossingInfo<K> where K : struct
    {
        public CrossingInfo(Task underlyingTask, K? requesterId, string jobId)
        {
            UnderlyingTask = underlyingTask;
            RequesterId = requesterId;
            JobId = jobId;
        }

        public Task UnderlyingTask { get; }

        public K? RequesterId { get; }

        public string JobId { get; }
    }
}
