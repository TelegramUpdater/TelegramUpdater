using TelegramUpdater;
using Xunit;

namespace TelegramUpdaterTests;

public class UpdaterBuilderTests
{
    [Fact]
    public void ZeroTestAsync()
    {
        var _updater = new UpdaterBuilder(Extensions.FakeBotToken)
            .StepOne()
            .StepTwo()
            .StepThree();
    }
}
