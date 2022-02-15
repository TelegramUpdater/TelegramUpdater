using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    /// <summary>
    /// An update container for <see cref="Update.Message"/> only.
    /// </summary>
    public sealed class MessageContainer : UpdateContainerAbs<Message>
    {
        internal MessageContainer(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
            : base(x => x.Message, updater, shiningInfo)
        {
        }
    }
}
