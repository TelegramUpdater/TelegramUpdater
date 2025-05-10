using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using Xunit;

namespace TelegramUpdaterTests
{
    class FakeWriter : AbstractUpdateWriter
    {
        protected override Task Execute(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }

    class MyMessageHandler : MessageHandler
    {
        protected override Task HandleAsync(MessageContainer updateContainer)
        {
            throw new NotImplementedException();
        }
    }

    class MyCallbackQueryHandler : CallbackQueryHandler
    {
        protected override Task HandleAsync(CallbackQueryContainer updateContainer)
        {
            throw new NotImplementedException();
        }
    }

    public class DetectAllowedUpdatesTest
    {
        [Fact]
        public async Task Test_1Async()
        {
            var testUpdater = new Updater(new TelegramBotClient(Extensions.FakeBotToken));

            testUpdater.AddScopedUpdateHandler<MyMessageHandler>(UpdateType.Message);
            testUpdater.AddScopedUpdateHandler<MyMessageHandler>(UpdateType.EditedMessage);
            testUpdater.AddHandler<MyCallbackQueryHandler>();

            testUpdater.AddSingletonUpdateHandler(
                new TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.MessageHandler(_T));

            // Writer's BeforeExecution method will fill up allowed updates.
            await testUpdater.Start<FakeWriter>();

            Assert.True(testUpdater.AllowedUpdates?.SequenceEqual(
                [UpdateType.Message, UpdateType.EditedMessage, UpdateType.CallbackQuery]));
        }

        [Fact]
        public async Task Test_2Async()
        {
            var testUpdater = new Updater(new TelegramBotClient(Extensions.FakeBotToken),
                new UpdaterOptions(allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]));

            await testUpdater.Start<FakeWriter>();

            Assert.True(testUpdater.AllowedUpdates?.SequenceEqual(
                [UpdateType.Message, UpdateType.CallbackQuery]));
        }

        private Task _T(IContainer<Message> arg)
        {
            throw new NotImplementedException();
        }
    }
}
