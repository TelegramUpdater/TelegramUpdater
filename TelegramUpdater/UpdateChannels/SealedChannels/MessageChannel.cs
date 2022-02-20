using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateChannels.AbstractChannels;

namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    /// <summary>
    /// A channel for <see cref="Update.Message"/>.
    /// </summary>
    public sealed class MessageChannel : AnyChannel<Message>
    {
        /// <summary>
        /// Create an instance of update channels for <see cref="Update.Message"/>
        /// </summary>
        /// <param name="filter">Filter to choose the right update to channel.</param>
        public MessageChannel(TimeSpan timeOut, Filter<Message>? filter = default)
            : base(UpdateType.Message, x => x.Message, timeOut, filter)
        {
        }
    }
}
