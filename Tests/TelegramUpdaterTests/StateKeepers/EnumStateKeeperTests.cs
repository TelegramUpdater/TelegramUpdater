using System;
using TelegramUpdater.StateKeeping.StateKeepers;
using Xunit;

namespace TelegramUpdaterTests.StateKeepers
{
    public class EnumStateKeeperTests
    {
        internal sealed class EnumStateKeeper<TEnum> : AbstractEnumStateKeeper<TEnum, long>
            where TEnum : struct, Enum
        {
            protected override Func<long, long> KeyResolver => x => x;
        }

        internal enum MySteps : int
        {
            Start,
            Step1,
            Step2,
            Step3,
            Finish
        }

        [Fact]
        public void EnumStateKeeperTest()
        {
            var keeper = new EnumStateKeeper<MySteps>();

            keeper.InitializeState(1);
            Assert.True(keeper.HasState(1, MySteps.Start));
            Assert.Equal(MySteps.Start, keeper.GetState(1));

            Assert.True(keeper.TryMoveForward(1, out MySteps? newState));
            Assert.Equal(MySteps.Step1, newState);
            Assert.True(keeper.HasState(1, MySteps.Step1));
            Assert.Equal(MySteps.Step1, keeper.GetState(1));

            Assert.True(keeper.TryMoveForward(1, out newState));
            Assert.Equal(MySteps.Step2, newState);
            Assert.True(keeper.HasState(1, MySteps.Step2));
            Assert.Equal(MySteps.Step2, keeper.GetState(1));

            Assert.True(keeper.TryMoveForward(1, out newState));
            Assert.Equal(MySteps.Step3, newState);
            Assert.True(keeper.HasState(1, MySteps.Step3));
            Assert.Equal(MySteps.Step3, keeper.GetState(1));

            Assert.True(keeper.TryMoveForward(1, out newState));
            Assert.Equal(MySteps.Finish, newState);
            Assert.True(keeper.HasState(1, MySteps.Finish));
            Assert.Equal(MySteps.Finish, keeper.GetState(1));

            Assert.False(keeper.TryMoveForward(1, out newState));
            Assert.Null(newState);
            Assert.True(keeper.HasState(1, MySteps.Finish));
            Assert.Equal(MySteps.Finish, keeper.GetState(1));

            Assert.True(keeper.TryMoveBackward(1, out newState));
            Assert.Equal(MySteps.Step3, newState);
            Assert.True(keeper.HasState(1, MySteps.Step3));
            Assert.Equal(MySteps.Step3, keeper.GetState(1));

            Assert.True(keeper.TryMoveBackward(1, out newState));
            Assert.Equal(MySteps.Step2, newState);
            Assert.True(keeper.HasState(1, MySteps.Step2));
            Assert.Equal(MySteps.Step2, keeper.GetState(1));

            Assert.True(keeper.TryMoveBackward(1, out newState));
            Assert.Equal(MySteps.Step1, newState);
            Assert.True(keeper.HasState(1, MySteps.Step1));
            Assert.Equal(MySteps.Step1, keeper.GetState(1));

            Assert.True(keeper.TryMoveBackward(1, out newState));
            Assert.Equal(MySteps.Start, newState);
            Assert.True(keeper.HasState(1, MySteps.Start));
            Assert.Equal(MySteps.Start, keeper.GetState(1));

            Assert.False(keeper.TryMoveBackward(1, out newState));
            Assert.Null(newState);
            Assert.True(keeper.HasState(1, MySteps.Start));
            Assert.Equal(MySteps.Start, keeper.GetState(1));
        }
    }
}
