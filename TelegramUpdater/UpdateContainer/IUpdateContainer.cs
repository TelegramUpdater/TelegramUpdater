using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer
{
    /// <summary>
    /// Base class for update containers. ( Used while handling updates )
    /// </summary>
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
        /// Processing info for this update.
        /// </summary>
        ShiningInfo<long, Update> ShiningInfo { get; }

        /// <summary>
        /// <see cref="ITelegramBotClient"/> which is resposeable for this container.
        /// </summary>
        public ITelegramBotClient BotClient { get; }
    }
}
