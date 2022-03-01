using System;
using Telegram.Bot.Types;
using TelegramUpdater.Filters;
using Xunit;

namespace TelegramUpdaterTests
{
    public class CommandFilterTests
    {
        [Fact]
        public void Test_JoinArgs_1()
        {
            var filter = new CommandFilter(
                "test",
                new CommandFilterOptions(joinArgsFormIndex: 0));

            filter.TheyShellPass(new Message { Text = "/test hello world" });

            if (filter.ExtraData is null) throw new Exception("ExtraData is null");

            var args = (filter.ExtraData["args"]) as string[];

            Assert.Equal(args![0], "hello world");
        }
    }
}