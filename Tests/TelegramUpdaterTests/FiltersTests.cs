using System;
using System.Linq;
using TelegramUpdater;
using Xunit;

namespace TelegramUpdaterTests
{
    class IntFilter : Filter<int>
    {
        public IntFilter(Func<IUpdater, int, bool> filter) : base(filter)
        {
        }
    }

    public class FiltersTests
    {
        [Fact]
        public void AndFilterTest_1()
        {
            var filter = new IntFilter((_, x)=> true) & new IntFilter((_, x) => false);

            Assert.IsType<AndFilter<int>> (filter);
        }

        [Fact]
        public void AndFilterTest_2()
        {
            var filter = new Filter<int>((_, x) => true) & new Filter<int>((_, x) => false);

            Assert.IsType<AndFilter<int>>(filter);
        }

        [Fact]
        public void AndFilterTest_3()
        {
            var filter = new Filter<int>((_, x) => true) & new Filter<int>((_, x) => false);

            Assert.False(filter.TheyShellPass(null!, 0));
        }

        [Fact]
        public void AndFilterTest_4()
        {
            var filter = new Filter<int>((_, x) => true) & new Filter<int>((_, x) => true);

            Assert.True(filter.TheyShellPass(null!, 0));
        }

        [Fact]
        public void OrFilterTest_1()
        {
            var filter = new IntFilter((_, x) => true) | new IntFilter((_, x) => false);

            Assert.IsType<OrFilter<int>>(filter);
        }

        [Fact]
        public void OrFilterTest_2()
        {
            var filter = new Filter<int>((_, x) => true) | new Filter<int>((_, x) => false);

            Assert.True(filter.TheyShellPass(null!, 0));
        }

        [Fact]
        public void OrFilterTest_3()
        {
            var filter = new Filter<int>((_, x) => true) | new Filter<int>((_, x) => false);

            Assert.True(filter.TheyShellPass(null!, 0));
        }

        [Fact]
        public void OrFilterTest_4()
        {
            var filter = new Filter<int>((_, x) => false) | new Filter<int>((_, x) => false);

            Assert.False(filter.TheyShellPass(null!, 0));
        }

        [Fact]
        public void CombinedFilterTest_1()
        {
            var filter = new Filter<int>((_, x) => false) | new Filter<int>((_, x) => false);
            var filter_2 = filter | new Filter<int>((_, x) => true);

            Assert.True(filter_2.TheyShellPass(null!, 0));
        }

        [Fact]
        public void CombinedFilterTest_2()
        {
            var filter = new Filter<int>((_, x) => false) | new Filter<int>((_, x) => false);
            var filter_2 = filter & new Filter<int>((_, x) => true);

            Assert.False(filter_2.TheyShellPass(null!, 0));
        }

        [Fact]
        public void IsFilterTest_1()
        {
            var filter = new Filter<int>((_, x) => false) | new Filter<int>((_, x) => false);

            var isFilter = filter.GetType().IsFilter();
            Assert.True(isFilter);
        }

        [Fact]
        public void IsFilterTest_2()
        {
            var filter = new Filter<int>((_, x) => false);

            var isFilter = filter.GetType().IsFilter();
            Assert.True(isFilter);
        }

        class MyFilter : Filter<int>
        {
            public MyFilter() : base((_, x) => x == 10)
            {
            }
        }

        [Fact]
        public void IsFilterTest_3()
        {
            var isFilter = typeof(MyFilter).IsFilter();
            Assert.True(isFilter);
        }

        [Fact]
        public void IsFilterTest_4()
        {
            var myFilter = new MyFilter();

            var isFilter = myFilter.GetType().IsFilter();
            Assert.True(isFilter);
        }

        [Fact]
        public void IsFilterOfTypeTest_1()
        {
            var filter = new Filter<int>((_, x) => false);

            var isFilter = filter.GetType().IsFilterOfType(typeof(int));
            Assert.True(isFilter);
        }

        [Fact]
        public void IsFilterOfTypeTest_2()
        {
            var filter = new Filter<int>((_, x) => false) | new Filter<int>((_, x) => false);

            var isFilter = filter.GetType().IsFilterOfType(typeof(int));
            Assert.True(isFilter);
        }

        [Fact]
        public void IsFilterOfTypeTest_3()
        {
            var isFilter = typeof(MyFilter).IsFilterOfType(typeof(int));
            Assert.True(isFilter);
        }

        [Fact]
        public void Discovery_Test_1()
        {
            var filter = new Filter<int>((_, x) => x == 10);

            var dicovered = filter.DiscoverNestedFilters();

            Assert.Equal(dicovered.First(), filter);
        }

        [Fact]
        public void Discovery_Test_3()
        {
            var filter1 = new Filter<int>((_, x) => x == 10);
            var filter2 = new Filter<int>((_, x) => x == 11);
            var filter3 = new Filter<int>((_, x) => x == 12);
            var filter4 = filter1 | filter2;
            var filter5 = filter4 & filter3;

            var dicovered = filter5.DiscoverNestedFilters();

            Assert.Contains(filter1, dicovered);
            Assert.Contains(filter2, dicovered);
            Assert.Contains(filter3, dicovered);
        }
    }
}