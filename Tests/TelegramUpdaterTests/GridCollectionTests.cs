using TelegramUpdater.Helpers;
using Xunit;

namespace TelegramUpdaterTests
{
    public class GridCollectionTests
    {
        [Fact]
        public void Test_1()
        {
            var grid = new GridCollection<int>
            {
                10,
                10,
                10,
                10
            };

            Assert.Equal(4, grid.Count);
            Assert.Equal(1, grid.RowsCount);
        }

        [Fact]
        public void Test_2()
        {
            var grid = new GridCollection<int>(2)
            {
                10,
                10,
                10,
                10
            };

            Assert.Equal(4, grid.Count);
            Assert.Equal(2, grid.RowsCount);
        }
    }
}
