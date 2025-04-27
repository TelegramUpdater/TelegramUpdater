namespace TelegramUpdater.UpdateChannels;

/// <summary>
/// An abstract class for channel updates.
/// </summary>
/// <typeparam name="T">Type of update to channel</typeparam>
public abstract class AbstractChannel<T> : IGenericUpdateChannel<T>
    where T : class
{
    private readonly Func<Update, T?> _getT;
    private readonly IFilter<UpdaterFilterInputs<T>>? _filter;

    /// <summary>
    /// An abstract class for channel updates.
    /// </summary>
    /// <param name="updateType">Type of update.</param>
    /// <param name="getT">
    /// A function to select the right update from <see cref="Update"/>
    /// </param>
    /// <param name="filter">Filter.</param>
    /// <param name="timeOut">Time out to wait for the channel.</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected AbstractChannel(
        UpdateType updateType,
        Func<Update, T?> getT,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<T>>? filter)
    {
        if (timeOut == default)
            throw new ArgumentException("Use a valid time out.", nameof(timeOut));

        if (updateType == UpdateType.Unknown)
            throw new ArgumentException(
                $"There's nothing unknown here! {nameof(updateType)}", nameof(updateType));

        TimeOut = timeOut;
        _filter = filter;
        UpdateType = updateType;
        _getT = getT ?? throw new ArgumentNullException(nameof(getT));
    }

    /// <inheritdoc/>
    public UpdateType UpdateType { get; }

    /// <inheritdoc/>
    public TimeSpan TimeOut { get; }

    IReadOnlyDictionary<string, object>? IGenericUpdateChannel<T>.ExtraData
        => _filter?.ExtraData;

    /// <inheritdoc/>
    public T? GetActualUpdate(Update update) => _getT(update);

    /// <inheritdoc/>
    private bool ShouldChannel(UpdaterFilterInputs<T> inputs)
    {
        if (_filter is null) return true;

        return _filter.TheyShellPass(inputs);
    }

    /// <summary>
    /// If this update should be channeled.
    /// </summary>
    public bool ShouldChannel(UpdaterFilterInputs<Update> inputs)
    {
        if (inputs.Input.Type != UpdateType) return false;

        var insider = GetActualUpdate(inputs.Input);

        if (insider == null) return false;

        return ShouldChannel(new UpdaterFilterInputs<T>(inputs.Updater, insider));
    }
}
