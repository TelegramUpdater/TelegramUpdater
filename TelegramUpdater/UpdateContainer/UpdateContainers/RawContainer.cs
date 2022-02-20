using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    /// <summary>
    /// Raw container has raw <see cref="Update"/> only, inner update must be decieded manually.
    /// </summary>
    public sealed class RawContainer : IUpdateContainer
    {
        /// <summary>
        /// Create an instance of <see cref="RawContainer"/>
        /// </summary>
        /// <param name="updater">The <see cref="IUpdater"/> instance</param>
        /// <param name="shiningInfo">Shining info about the received update,</param>
        public RawContainer(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        {
            Updater = updater;
            ShiningInfo = shiningInfo;
        }

        /// <inheritdoc/>
        public IUpdater Updater { get; }

        /// <inheritdoc/>
        public Update Container => ShiningInfo.Value;

        /// <inheritdoc/>
        public ShiningInfo<long, Update> ShiningInfo { get; }

        /// <inheritdoc/>
        public ITelegramBotClient BotClient => Updater.BotClient;
    }
}
