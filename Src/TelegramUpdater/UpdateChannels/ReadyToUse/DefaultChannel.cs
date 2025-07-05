namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// Channel that can be used to get updates of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of expecting update.</typeparam>
/// <param name="updateType">Type of the update.</param>
/// <param name="getT">Optionally resolve inner update.</param>
/// <param name="timeOut">Time out that channel should wait for update.</param>
/// <param name="filter">Filter the incoming <typeparamref name="T"/>.</param>
public class DefaultChannel<T>(
    UpdateType updateType, TimeSpan timeOut, Func<Update, T?>? getT = default, IFilter<UpdaterFilterInputs<T>>? filter = default)
    : AbstractUpdateChannel<T>(
        updateType: updateType,
        timeOut: timeOut,
        getT: getT,
        filter: filter) where T : class
{
}
