using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers
{
    /// <summary>
    /// Interface for normal update handler ( known as singleton handlers )
    /// </summary>
    public interface ISingletonUpdateHandler : IUpdateHandler
    {
        /// <summary>
        /// Type of update.
        /// </summary>
        UpdateType UpdateType { get; }

        /// <summary>
        /// Checks if an update can be handled in this <see cref="ISingletonUpdateHandler"/>
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        public bool ShouldHandle(Update update);
    }
}
