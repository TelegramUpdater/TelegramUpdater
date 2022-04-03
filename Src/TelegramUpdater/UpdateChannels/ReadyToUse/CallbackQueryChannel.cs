namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// An <see cref="IGenericUpdateChannel{T}"/> for <see cref="UpdateType.CallbackQuery"/>.
/// </summary>
public sealed class CallbackQueryChannel : AnyChannel<CallbackQuery>
{
    /// <summary>
    /// Initialize a new instance of <see cref="CallbackQueryChannel"/>
    /// to use as <see cref="IGenericUpdateChannel{T}"/>.
    /// </summary>
    /// <param name="timeOut">Timeout to wait for channel.</param>
    /// <param name="filter">Filter suitable update to channel within <paramref name="timeOut"/>.</param>
    public CallbackQueryChannel(TimeSpan timeOut,
                                IFilter<CallbackQuery>? filter = default)
        : base(UpdateType.CallbackQuery,
               x => x.CallbackQuery,
               timeOut,
               filter)
    {
    }
}
