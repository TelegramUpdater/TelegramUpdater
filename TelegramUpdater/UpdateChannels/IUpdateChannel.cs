using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels
{
    /// <summary>
    /// Base interface for channels.
    /// </summary>
    public interface IUpdateChannel
    {
        /// <summary>
        /// Update type.
        /// </summary>
        public UpdateType UpdateType { get; }

        /// <summary>
        /// If this update should be channeled.
        /// </summary>
        public bool ShouldChannel(Update update);
    }
}
