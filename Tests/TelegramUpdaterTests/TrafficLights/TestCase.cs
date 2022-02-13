using System.Threading.Tasks;

namespace TelegramUpdaterTests.TrafficLights
{
    public class TestCase
    {
        public TestCase(int id, int ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }

        public int Id { get; }

        public int OwnerId { get; }


        public async Task WaitingJob(int seconds)
        {
            System.Console.WriteLine($"Processing {Id}, from {OwnerId}");
            await Task.Delay(seconds * 1000);
        }
    }
}
