using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// Base class for update containers. ( Used while handling updates )
/// </summary>
public interface IUpdateContainer
{
    /// <summary>
    /// <see cref="TelegramUpdater.Updater"/> instance which is resposeable
    /// for this container.
    /// </summary>
    public IUpdater Updater { get; }

    /// <summary>
    /// The received update.
    /// </summary>
    public Update Container { get; }

    /// <summary>
    /// Processing info for this update.
    /// </summary>
    ShiningInfo<long, Update> ShiningInfo { get; }

    /// <summary>
    /// <see cref="ITelegramBotClient"/> which is resposeable for this container.
    /// </summary>
    public ITelegramBotClient BotClient { get; }

    /// <summary>
    /// Container may contain extra data based on the filter applied on handler.
    /// You can find that data here.
    /// </summary>
    /// <remarks>
    /// Eg: a <see cref="FilterCutify.OnCommand(string[])"/> may insert an string array with key <c>args</c>.
    /// </remarks>
    /// <param name="key">Data key, based on applied filter.</param>
    /// <returns>
    /// An object that you may need to cast.
    /// <para>Eg: var args = (string[])container["args"];</para>
    /// </returns>
    public object this[string key] { get; }

    /// <summary>
    /// Checks if a key is available to fetch from <see cref="this[string]"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><see langword="true"/> if the key is available.</returns>
    public bool ContainsKey(string key);
}
