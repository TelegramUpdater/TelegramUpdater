using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramUpdater.TrafficLights;
using Xunit;

namespace TelegramUpdaterTests.TrafficLights
{
    public class TrafficLightTests
    {
        [Theory]
        [InlineData(2)]
        public async Task ShouldBeOneByOne(int tasksCount)
        {
            TrafficLight<TestCase, int> trafficLight = new(x => x.OwnerId);

            var _jobs = Enumerable.Range(0, tasksCount).Select(x => new TestCase(x, 10000));
            var _tasks = new List<Task>();

            var start = DateTime.Now;

            foreach (var job in _jobs)
            {
                _tasks.Add(Task.Run(async () =>
                {
                    await trafficLight.AwaitYellowLight(job);

                     // It should take 3 secs.
                    var info = trafficLight.StartCrossingTask(job, job.WaitingJob(3));
                    await info.UnderlyingTask;

                    trafficLight.FinishCrossing(info);
                }));
            }

            await Task.WhenAll(_tasks);

            var timeTaken = (DateTime.Now - start).TotalSeconds;
            Assert.True(timeTaken >= 3 * tasksCount,
                $"They are not one by one, {tasksCount} took {timeTaken} but is should be at least {tasksCount*3}");
        }
    }
}
