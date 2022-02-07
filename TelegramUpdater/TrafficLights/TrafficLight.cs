using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TelegramUpdater.TrafficLights
{
    public class TrafficLight<T, K> where K : struct where T : class
    {
        private readonly ConcurrentDictionary<string, Task> _crossingCars;
        private readonly ConcurrentDictionary<K, string> _requesterMap;
        private readonly int _maxDegreeOfParallelism;
        private readonly Func<T, K> _getRequester;

        public TrafficLight(Func<T, K> getRequester, int? maxDegreeOfParallelism = default)
        {
            _crossingCars = new ConcurrentDictionary<string, Task>();
            _requesterMap = new ConcurrentDictionary<K, string>();
            _getRequester = getRequester;
            _maxDegreeOfParallelism = maxDegreeOfParallelism?? Environment.ProcessorCount;
        }

        /// <summary>
        /// The maximum number of concurrent update handling tasks
        /// </summary>
        public int MaxDegreeOfParallelism => _maxDegreeOfParallelism;

        /// <summary>
        /// Onginig processes count.
        /// </summary>
        public int CrossingCount => _crossingCars.Count;

        public async Task AwaitGreenLight()
        {
            if (_crossingCars.Count >= _maxDegreeOfParallelism)
            {
                await Task.WhenAny(_crossingCars.Values);
            }
        }

        public async Task AwaitYellowLight(T obj)
        {
            var requesterId = _getRequester(obj);

            if (_requesterMap.ContainsKey(requesterId))
            {
                await _crossingCars[_requesterMap[requesterId]];
            }
        }

        public CrossingInfo<K> StartCrossingTask(Task crossWrap)
        {
            var jobId = crossWrap.Id.ToString();

            _crossingCars.TryAdd(jobId, crossWrap);
            return new CrossingInfo<K>(crossWrap, null, jobId);
        }

        public CrossingInfo<K> StartCrossingTask(T obj, Task crossWrap)
        {
            var jobId = crossWrap.Id.ToString();
            K requesterId = _getRequester(obj);

            _crossingCars.TryAdd(jobId, crossWrap);
            _requesterMap.TryAdd(requesterId, jobId);
            return new CrossingInfo<K>(crossWrap, requesterId, jobId);
        }

        public void FinishCrossing(CrossingInfo<K> crossingInfo)
        {
            _crossingCars.TryRemove(crossingInfo.JobId, out _);
            if (crossingInfo.RequesterId != null)
            {
                _requesterMap.TryRemove(crossingInfo.RequesterId.Value, out _);
            }
        }
    }
}
