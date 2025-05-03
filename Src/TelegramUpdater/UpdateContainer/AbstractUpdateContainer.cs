using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// A container for incoming updates, which contains
/// <typeparamref name="T"/> as your update.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
/// <remarks>
/// Create a new instance of <see cref="AbstractUpdateContainer{T}"/>
/// </remarks>
/// <param name="insiderResolver"></param>
/// <param name="input"></param>
/// <param name="extraObjects"></param>
public abstract class AbstractUpdateContainer<T>(
    Func<Update, T?> insiderResolver,
    HandlerInput input,
    IReadOnlyDictionary<string, object>? extraObjects = default) : IContainer<T>
    where T : class
{
    private readonly Func<Update, T?> _insiderResolver = insiderResolver;
    private readonly IReadOnlyDictionary<string, object> _extraObjects = extraObjects ?? new Dictionary<string, object>(StringComparer.Ordinal);

    /// <inheritdoc/>
    public object this[string key] => _extraObjects[key];

    /// <summary>
    /// Actual update. ( inner update, like <see cref="Update.Message"/> ) 
    /// </summary>
    public T Update
    {
        get
        {
            var inner = _insiderResolver(Container);

            return inner ?? throw new InvalidOperationException(
                    $"Inner update should not be null!" +
                    "Expected {typeof(T)} is {GetType()}");
        }
    }

    /// <inheritdoc/>
    public HandlerInput Input { get; init; } = input;

    /// <inheritdoc/>
    public IUpdater Updater => Input.Updater;

    /// <inheritdoc/>
    public Update Container => ShiningInfo.Value;

    /// <inheritdoc/>
    public ShiningInfo<long, Update> ShiningInfo => Input.ShiningInfo;

    /// <inheritdoc/>
    public ITelegramBotClient BotClient => Updater.BotClient;

    /// <inheritdoc/>
    public bool ContainsKey(string key) => _extraObjects.ContainsKey(key);
}
