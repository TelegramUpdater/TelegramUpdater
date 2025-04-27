using System;
using TelegramUpdater;
using Xunit;
using Xunit.Abstractions;

namespace TelegramUpdaterTests
{
    public class FilterWithEyeTests
    {
        private readonly ITestOutputHelper output;

        public FilterWithEyeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        class DickerMin(ITestOutputHelper output) : Filter<int>
        {
            public override bool EyeAssistant => false;

            protected override bool TheyShellPass(int input)
            {
                output.WriteLine($"Processed {GetType().Name}");
                return input >= 0;
            }
        }

        class DickerMax(ITestOutputHelper output) : Filter<int>
        {
            public override bool EyeAssistant => true;

            protected override bool TheyShellPass(int input)
            {
                output.WriteLine($"Processed {GetType().Name}");
                return input <= 10;
            }
        }

        [Fact]
        public void FirstTest()
        {
            var eye = new FilterEye();

            var input = -5;

            Assert.False((new DickerMax(output) & new DickerMin(output)).Evaluate(input, eye));
            Assert.True((new DickerMin(output) | new DickerMax(output)).Evaluate(input, eye));
        }
    }
}
