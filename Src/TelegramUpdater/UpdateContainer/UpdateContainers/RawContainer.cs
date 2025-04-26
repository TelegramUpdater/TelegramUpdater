using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Raw container has raw <see cref="Update"/> only,
/// inner update must be decided manually.
/// </summary>
/// <remarks>
/// Create an instance of <see cref="RawContainer"/>
/// </remarks>
/// <param name="updater">The <see cref="IUpdater"/> instance</param>
/// <param name="shiningInfo">
/// Shining info about the received update.
/// </param>
/// <param name="extraObjects">
/// A dictionary of extra data for this container.
/// </param>
public sealed class RawContainer(
    IUpdater updater,
    ShiningInfo<long, Update> shiningInfo,
    IReadOnlyDictionary<string, object>? extraObjects = default) : IUpdateContainer
{
    private readonly IReadOnlyDictionary<string, object> _extraObjects = extraObjects
        ?? new Dictionary<string, object>(StringComparer.Ordinal);

    /// <inheritdoc/>
    public object this[string key] => _extraObjects[key];

    /// <inheritdoc/>
    public IUpdater Updater { get; } = updater;

    /// <inheritdoc/>
    public Update Container => ShiningInfo.Value;

    /// <inheritdoc/>
    public ShiningInfo<long, Update> ShiningInfo { get; } = shiningInfo;

    /// <inheritdoc/>
    public ITelegramBotClient BotClient => Updater.BotClient;

    /// <inheritdoc/>
    public bool ContainsKey(string key) => _extraObjects.ContainsKey(key);
}
