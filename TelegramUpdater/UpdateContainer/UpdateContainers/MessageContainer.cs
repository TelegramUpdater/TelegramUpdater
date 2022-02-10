using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    public sealed class MessageContainer : UpdateContainerAbs<Message>
    {
        public MessageContainer(IUpdater updater, Update insider)
            : base(x => x.Message, updater, insider)
        {
        }
    }
}
