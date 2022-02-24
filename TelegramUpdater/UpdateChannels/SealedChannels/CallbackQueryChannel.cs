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
        /// <param name="timeOut">Waiting for update timeout.</param>
        /// <param name="filter">A filter to select the right update.</param>
        public CallbackQueryChannel(TimeSpan timeOut, Filter<CallbackQuery>? filter = default)
            : base(UpdateType.CallbackQuery, x => x.CallbackQuery, timeOut, filter)
        {
        }
    }
}
