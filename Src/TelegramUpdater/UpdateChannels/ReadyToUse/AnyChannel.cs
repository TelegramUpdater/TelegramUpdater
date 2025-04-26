namespace TelegramUpdater.UpdateChannels.ReadyToUse;

/// <summary>
/// Create channel for any type of <see cref="Update"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AnyChannel<T>(
    UpdateType updateType,
    Func<Update, T?> getT,
    TimeSpan timeOut,
    IFilter<T>? filter) : AbstractChannel<T>(updateType, getT, timeOut, filter) where T : class
{
}
