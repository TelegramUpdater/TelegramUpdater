namespace TelegramUpdater.UpdateChannels.SealedChannels
{
    /// <summary>
    /// A channel for <see cref="Update.InlineQuery"/>.
    /// </summary>
    public class InlineQueryChannel : AnyChannel<InlineQuery>
    {
        /// <summary>
        /// Create an instance of inline query channel.
        /// </summary>
        /// <param name="timeOut">Waiting for update timeout.</param>
        /// <param name="filter">A filter to select the right update.</param>
        public InlineQueryChannel(TimeSpan timeOut, IFilter<InlineQuery>? filter)
            : base(UpdateType.InlineQuery, x => x.InlineQuery, timeOut, filter)
        {
        }
    }
}
