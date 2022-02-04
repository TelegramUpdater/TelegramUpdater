using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    public sealed class MessageContainer : UpdateContainerAbs<Message>
    {
        public MessageContainer(Updater updater, Update insider, ITelegramBotClient botClient)
            : base(x=> x.Message, updater, insider, botClient)
        {
        }
    }
}
