using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using Xunit;

namespace TelegramUpdaterTests
{
    class FakeWriter : UpdateWriterAbs
    {
        public override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }

    class MyMessageHandler : MessageHandler
    {
        protected override Task HandleAsync(IContainer<Message> updateContainer)
        {
            throw new NotImplementedException();
        }
    }

    class MyCallbackQueryHandler : CallbackQueryHandler
    {
        protected override Task HandleAsync(IContainer<CallbackQuery> updateContainer)
        {
            throw new NotImplementedException();
        }
    }

    public class DetectAllowedUpdatesTest
    {
        private readonly string FakeBotToken = "123456789:ABCdefghij8kLM2nQrisT7_v4TMAKdiHm9T0";

        [Fact]
        public async Task Test_1Async()
        {
            var testUpdater = new Updater(new TelegramBotClient(FakeBotToken));

            testUpdater.AddScopedUpdateHandler<MyMessageHandler, Message>();
            testUpdater.AddScopedUpdateHandler<MyCallbackQueryHandler, CallbackQuery>();

            testUpdater.AddSingletonUpdateHandler(
                new TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.MessageHandler(_T));

            await testUpdater.StartAsync<FakeWriter>();

            Assert.True(testUpdater.AllowedUpdates.SequenceEqual(
                [UpdateType.Message, UpdateType.CallbackQuery]));
        }

        [Fact]
        public async Task Test_2Async()
        {
            var testUpdater = new Updater(new TelegramBotClient(FakeBotToken),
                new UpdaterOptions(allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]));

            await testUpdater.StartAsync<FakeWriter>();

            Assert.True(testUpdater.AllowedUpdates.SequenceEqual(
                [UpdateType.Message, UpdateType.CallbackQuery]));
        }

        private Task _T(IContainer<Message> arg)
        {
            throw new NotImplementedException();
        }
    }
}
