namespace TelegramUpdater.UpdateChannels;

/// <summary>
/// Base interface for channels.
/// </summary>
public interface IUpdateChannel
{
    /// <summary>
    /// Update type.
    /// </summary>
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Time out to wait for a channel.
    /// </summary>
    public TimeSpan TimeOut { get; }

    /// <summary>
    /// If this update should be channeled.
    /// </summary>
    public bool ShouldChannel(Update update);
}
