namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// A channel for <see cref="Update.Message"/>.
/// </summary>
public sealed class MessageChannel : AnyChannel<Message>
{
    /// <summary>
    /// Create an instance of update channels for <see cref="Update.Message"/>
    /// </summary>
    /// <param name="timeOut">Waiting for update timeout.</param>
    /// <param name="filter">A filter to select the right update.</param>
    public MessageChannel(TimeSpan timeOut,
                          IFilter<Message>? filter = default)
        : base(UpdateType.Message,
               x => x.Message,
               timeOut,
               filter)
    {
    }
}
