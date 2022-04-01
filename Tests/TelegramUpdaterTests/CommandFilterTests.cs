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

            Assert.Equal("hello world", args![0]);
        }

        [Theory]
        [InlineData("/test")]
        [InlineData("/Test")]
        [InlineData("/Test hello world")]
        [InlineData("/test@testbot")]
        [InlineData("/test@TestBot")]
        [InlineData("/test@TestBot hello world")]
        public void Test_Command_With_Username_1(string command)
        {
            var filter = new CommandFilter(
                "test",
                new CommandFilterOptions(botUsername: "testbot"));

            var message = new Message { Text = command };

            Assert.True(filter.TheyShellPass(message));
        }

        [Theory]
        [InlineData("/test@testbot2")]
        [InlineData("!test@TestBot")]
        [InlineData("/test@TestBothello world")]
        public void Test_Command_With_Username_2(string command)
        {
            var filter = new CommandFilter(
                "test",
                new CommandFilterOptions(botUsername: "testbot"));

            var message = new Message { Text = command };

            Assert.False(filter.TheyShellPass(message));
        }

        [Theory]
        [InlineData("/test")]
        [InlineData("/Test")]
        [InlineData("/Test hello world")]
        [InlineData("/test@testbot")]
        [InlineData("/test@TestBot")]
        [InlineData("/test@BlahBot")]
        [InlineData("/test@TestBot hello world")]
        public void Test_Command_With_No_Username_1(string command)
        {
            var filter = new CommandFilter(
                "test",
                new CommandFilterOptions());

            var message = new Message { Text = command };

            Assert.True(filter.TheyShellPass(message));
        }
    }
}