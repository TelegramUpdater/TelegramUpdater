using Telegram.Bot.Types.Enums;
using TelegramUpdater.Helpers;
using Xunit;

namespace TelegramUpdaterTests
{
    public class ChatTypeFlagsTests
    {
        [Fact]
        public void Test_1()
        {
            var flags = ChatTypeFlags.Group | ChatTypeFlags.SuperGroup;

            Assert.True(ChatType.Group.IsCorrect(flags));
            Assert.True(ChatType.Supergroup.IsCorrect(flags));
            Assert.False(ChatType.Private.IsCorrect(flags));
            Assert.False(ChatType.Channel.IsCorrect(flags));
            Assert.False(ChatType.Sender.IsCorrect(flags));
        }

        [Fact]
        public void Test_2()
        {
            var flags = ChatTypeFlags.Group & ChatTypeFlags.SuperGroup;

            Assert.False(ChatType.Group.IsCorrect(flags));
        }

        [Fact]
        public void Test_3()
        {
            var flags = ChatTypeFlags.Group; ;

            Assert.True(ChatType.Group.IsCorrect(flags));
            Assert.False(ChatType.Supergroup.IsCorrect(flags));
            Assert.False(ChatType.Private.IsCorrect(flags));
            Assert.False(ChatType.Channel.IsCorrect(flags));
            Assert.False(ChatType.Sender.IsCorrect(flags));
        }
    }
}
