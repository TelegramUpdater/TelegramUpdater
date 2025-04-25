using System;
using TelegramUpdater.StateKeeping.StateKeepers;
using Xunit;

namespace TelegramUpdaterTests.StateKeepers;

public class NumericStateKeeperTests
{
    internal class NumericStateKeeperUnbound : AbstractNumericStateKeeper<long, long>
    {
        protected override Func<long, long> KeyResolver => x => x;
    }

    internal class NumericStateKeeperBound : AbstractNumericStateKeeper<long, long>
    {
        public NumericStateKeeperBound(Range stateRange)
        {
            StateRange = stateRange;
        }

        protected override Func<long, long> KeyResolver => x => x;

        public override Range? StateRange { get; }
    }

    [Fact]
    public void TestNumericStateKeeperUnbound()
    {
        var keeper = new NumericStateKeeperUnbound();

        keeper.InitializeState(1);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, 0));
        Assert.Equal(0, keeper.GetState(1));

        Assert.True(keeper.TryMoveForward(1, out var fwdState));
        Assert.Equal(1, fwdState);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, 1));
        Assert.Equal(1, keeper.GetState(1));

        Assert.True(keeper.TryMoveBackward(1, out var bwdState));
        Assert.Equal(0, bwdState);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, 0));
        Assert.Equal(0, keeper.GetState(1));

        Assert.True(keeper.TryMoveBackward(1, out var negState));
        Assert.Equal(-1, negState);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, -1));
        Assert.Equal(-1, keeper.GetState(1));
    }

    [Fact]
    public void TestNumericStateKeeperBound()
    {
        var keeper = new NumericStateKeeperBound(0..5);

        keeper.InitializeState(1);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, 0));
        Assert.Equal(0, keeper.GetState(1));

        Assert.False(keeper.TryMoveBackward(1, out var negState));
        Assert.Null(negState);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, 0));
        Assert.Equal(0, keeper.GetState(1));

        keeper.SetState(1, 5);
        Assert.True(keeper.HasState(1, 5));

        keeper.SetState(1, 6);
        Assert.True(keeper.HasState(1, 5));

        Assert.False(keeper.TryMoveForward(1, out var exdState));
        Assert.Null(exdState);

        Assert.True(keeper.HasAnyState(1));
        Assert.True(keeper.HasState(1, 5));
        Assert.Equal(5, keeper.GetState(1));
    }
}
