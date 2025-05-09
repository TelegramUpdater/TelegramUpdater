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

            filter.TheyShellPass(new(null!, new Message { Text = "/test hello world" }, default, default, default, default));

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

            Assert.True(filter.TheyShellPass(new(null!, message, default, default, default, default)));
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

            Assert.False(filter.TheyShellPass(new(null!, message, default, default, default, default)));
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

            Assert.True(filter.TheyShellPass(new(null!, message, default, default, default, default)));
        }

        [Theory]
        [InlineData("/test", true)]
        [InlineData("/Test", false)]
        [InlineData("/Test hello world", false)]
        [InlineData("/test@testbot", true)]
        [InlineData("/test@TestBot", true)]
        [InlineData("/test@BlahBot", true)]
        [InlineData("/test@TestBot hello world", true)]
        public void Test_Command_Case_Sensetive(string command, bool shouldMatch)
        {
            var filter = new CommandFilter(
                "test",
                new CommandFilterOptions(caseSensitive: true));

            var message = new Message { Text = command };

            Assert.Equal(filter.TheyShellPass(new(null!, message, default, default, default, default)), shouldMatch);
        }

        [Theory]
        [InlineData("/start test", true)]
        [InlineData("/start Test", true)]
        [InlineData("/start Test world", true)]
        [InlineData("/start", false)]
        [InlineData("/start smth", false)]
        public void Test_Command_Exact_Args_1(string command, bool shouldMatch)
        {
            var filter = new CommandFilter(
                "start",
                new CommandFilterOptions(
                    ArgumentsMode.Require,
                    exactArgs: new[] { "test" }));

            var message = new Message { Text = command };

            Assert.Equal(filter.TheyShellPass(new(null!, message, default, default, default, default)), shouldMatch);
        }

        [Theory]
        [InlineData("/start test you me", true)]
        [InlineData("/start Test you", true)]
        [InlineData("/start Test world", false)]
        [InlineData("/start test", false)]
        [InlineData("/start smth", false)]
        public void Test_Command_Exact_Args_2(string command, bool shouldMatch)
        {
            var filter = new CommandFilter(
                "start",
                new CommandFilterOptions(
                    ArgumentsMode.Require,
                    exactArgs: new[] { "test", "you" }));

            var message = new Message { Text = command };

            Assert.Equal(filter.TheyShellPass(new(null!, message, default, default, default, default)), shouldMatch);
        }

        [Theory]
        [InlineData("/start test", true)]
        [InlineData("/start Test", false)]
        [InlineData("/start", false)]
        [InlineData("/start smth", false)]
        public void Test_Command_Exact_Args_Case_Sensetive(string command, bool shouldMatch)
        {
            var filter = new CommandFilter(
                "start",
                new CommandFilterOptions(
                    ArgumentsMode.Require,
                    caseSensitive: true,
                    exactArgs: new[] { "test" }));

            var message = new Message { Text = command };

            Assert.Equal(filter.TheyShellPass(new(null!, message, default, default, default, default)), shouldMatch);
        }
    }
}