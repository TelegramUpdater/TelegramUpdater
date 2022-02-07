using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    public abstract class ScopedMessageHandler : AbstractScopedHandler<Message>
    {
        protected ScopedMessageHandler(int group = 0) : base(x=> x.Message, group)
        {
        }

        protected override UpdateContainerAbs<Message> ContainerBuilder(
            Updater updater, ITelegramBotClient botClient, Update update)
                => new MessageContainer(updater, update, botClient);
    }
}
