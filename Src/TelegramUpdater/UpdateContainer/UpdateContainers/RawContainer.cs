using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Raw container has raw <see cref="Update"/> only,
/// inner update must be decided manually.
/// </summary>
/// <remarks>
/// Create an instance of <see cref="RawContainer"/>
/// </remarks>
/// <param name="input"></param>
/// <param name="extraObjects">
/// A dictionary of extra data for this container.
/// </param>
public sealed class RawContainer(
    HandlerInput input,
    IReadOnlyDictionary<string, object>? extraObjects = default)
    : IUpdateContainer
{
    private readonly IReadOnlyDictionary<string, object> _extraObjects = extraObjects
        ?? new Dictionary<string, object>(StringComparer.Ordinal);

    /// <inheritdoc/>
    public object this[string key] => _extraObjects[key];

    /// <inheritdoc/>
    public IUpdater Updater => Input.Updater;

    /// <inheritdoc/>
    public Update Container => ShiningInfo.Value;

    /// <inheritdoc/>
    public ShiningInfo<long, Update> ShiningInfo => Input.ShiningInfo;

    /// <inheritdoc/>
    public ITelegramBotClient BotClient => Updater.BotClient;

    /// <inheritdoc/>
    public HandlerInput Input => input;

    /// <inheritdoc/>
    public bool ContainsKey(string key) => _extraObjects.ContainsKey(key);
}
