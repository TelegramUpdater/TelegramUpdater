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

        public async Task<R> CreateCrossingTask<R>(Task<R> crossWrap)
        {
            var jobId = crossWrap.Id.ToString();

            _crossingCars.TryAdd(jobId, crossWrap);
            var result = await crossWrap;
            _crossingCars.TryRemove(jobId, out _);
            return result;
        }

        public async Task CreateCrossingTask(Task crossWrap)
        {
            var jobId = crossWrap.Id.ToString();

            _crossingCars.TryAdd(jobId, crossWrap);
            await crossWrap;
            _crossingCars.TryRemove(jobId, out _);
        }

        public async Task CreateCrossingTask(T obj, Task crossWrap)
        {
            var jobId = crossWrap.Id.ToString();
            K requesterId = _getRequester(obj);

            _crossingCars.TryAdd(jobId, crossWrap);
            _requesterMap.TryAdd(requesterId, jobId);
            await crossWrap;
            _crossingCars.TryRemove(jobId, out _);
            _requesterMap.TryRemove(requesterId, out _);
        }

        public async Task<R> CreateCrossingTask<R>(T obj, Task<R> crossWrap)
        {
            var jobId = crossWrap.Id.ToString();
            K requesterId = _getRequester(obj);

            _crossingCars.TryAdd(jobId, crossWrap);
            _requesterMap.TryAdd(requesterId, jobId);
            var result = await crossWrap;
            _crossingCars.TryRemove(jobId, out _);
            _requesterMap.TryRemove(requesterId, out _);
            return result;
        }
    }
}
