namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.InlineQuery"/>.
/// </summary>
public sealed class InlineQueryChannel : AnyChannel<InlineQuery>
{
    /// <summary>
    /// Initialize a new instance of <see cref="InlineQueryChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public InlineQueryChannel(TimeSpan timeOut,
                              IFilter<InlineQuery>? filter)
        : base(UpdateType.InlineQuery,
               x => x.InlineQuery,
               timeOut,
               filter)
    {
    }
}
