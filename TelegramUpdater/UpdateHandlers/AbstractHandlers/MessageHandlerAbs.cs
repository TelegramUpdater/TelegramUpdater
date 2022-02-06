using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.AbstractHandlers
{
    public abstract class MessageHandlerAbs : AbstractHandler<Message>
    {
        protected MessageHandlerAbs(int group) : base(group)
        {
        }

        protected override UpdateContainerAbs<Message> ContainerBuilder(
            Updater updater, ITelegramBotClient botClient, Update update)
                => new MessageContainer(updater, update, botClient);

        protected override Message? GetT(Update update) => update.Message;

        public override UpdateType UpdateType => UpdateType.Message;
    }
}
