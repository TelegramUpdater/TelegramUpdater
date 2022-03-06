using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Raw container has raw <see cref="Update"/> only,
/// inner update must be decided manually.
/// </summary>
public sealed class RawContainer : IUpdateContainer
{
    private readonly IReadOnlyDictionary<string, object> _extraObjects;

    /// <summary>
    /// Create an instance of <see cref="RawContainer"/>
    /// </summary>
    /// <param name="updater">The <see cref="IUpdater"/> instance</param>
    /// <param name="shiningInfo">
    /// Shining info about the received update.
    /// </param>
    /// <param name="extraObjects">
    /// A dictionary of extra data for this container.
    /// </param>
    public RawContainer(
        IUpdater updater,
        ShiningInfo<long, Update> shiningInfo,
        IReadOnlyDictionary<string, object>? extraObjects = default)
    {
        Updater = updater;
        ShiningInfo = shiningInfo;
        _extraObjects = extraObjects ?? new Dictionary<string, object>();
    }

    /// <inheritdoc/>
    public object this[string key] => _extraObjects[key];

    /// <inheritdoc/>
    public IUpdater Updater { get; }

    /// <inheritdoc/>
    public Update Container => ShiningInfo.Value;

    /// <inheritdoc/>
    public ShiningInfo<long, Update> ShiningInfo { get; }

    /// <inheritdoc/>
    public ITelegramBotClient BotClient => Updater.BotClient;

    /// <inheritdoc/>
    public bool ContainsKey(string key) => _extraObjects.ContainsKey(key);
}
