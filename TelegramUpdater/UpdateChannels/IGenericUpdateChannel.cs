namespace TelegramUpdater.UpdateChannels;

/// <summary>
/// A generic update channel here.
/// </summary>
/// <typeparam name="T">Type of update.</typeparam>
public interface IGenericUpdateChannel<T> : IUpdateChannel where T : class
{
    internal IReadOnlyDictionary<string, object>? ExtraData { get; }

    /// <summary>
    /// Get the actual update from this channel.
    /// </summary>
    /// <value></value>
    public T? GetActualUpdate(Update update);
}
