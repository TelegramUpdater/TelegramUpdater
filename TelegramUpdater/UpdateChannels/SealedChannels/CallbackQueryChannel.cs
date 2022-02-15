using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    /// <summary>
    /// A channel for <see cref="Update.CallbackQuery"/>.
    /// </summary>
    public sealed class CallbackQueryChannel : AnyChannel<CallbackQuery>
    {
        /// <summary>
        /// Create an instance of update channels for <see cref="Update.CallbackQuery"/>
        /// </summary>
        /// <param name="filter">Filter to choose the right update to channel.</param>
        public CallbackQueryChannel(Filter<CallbackQuery>? filter)
            : base(UpdateType.CallbackQuery, x => x.CallbackQuery, filter)
        {
        }
    }
}
