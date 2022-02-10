using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer
{
    public interface IUpdateContainer
    {
        /// <summary>
        /// <see cref="TelegramUpdater.Updater"/> instance which is resposeable for this container.
        /// </summary>
        public IUpdater Updater { get; }

        /// <summary>
        /// The received update.
        /// </summary>
        public Update Container { get; }

        /// <summary>
        /// <see cref="ITelegramBotClient"/> which is resposeable for this container.
        /// </summary>
        public ITelegramBotClient BotClient { get; }
    }
}
