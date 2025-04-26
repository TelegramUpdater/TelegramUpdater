using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// A container for incoming updates, which contains
/// <typeparamref name="T"/> as your update.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
public abstract class AbstractUpdateContainer<T> : IContainer<T>
    where T : class
{
    private readonly Func<Update, T?> _insiderResolver;
    private readonly IReadOnlyDictionary<string, object> _extraObjects;

    /// <summary>
    /// Create a new instance of <see cref="AbstractUpdateContainer{T}"/>
    /// </summary>
    /// <param name="insiderResolver"></param>
    /// <param name="updater"></param>
    /// <param name="shiningInsider"></param>
    /// <param name="extraObjects"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected AbstractUpdateContainer(
        Func<Update, T?> insiderResolver,
        IUpdater updater,
        ShiningInfo<long, Update> shiningInsider,
        IReadOnlyDictionary<string, object>? extraObjects = default)
    {
        Updater = updater ??
            throw new ArgumentNullException(nameof(updater));
        ShiningInfo = shiningInsider;
        Container = shiningInsider.Value;
        BotClient = updater.BotClient;
        _insiderResolver = insiderResolver ??
            throw new ArgumentNullException(nameof(insiderResolver));
        _extraObjects = extraObjects ?? new Dictionary<string, object>(StringComparer.Ordinal);
    }

    /// <summary>
    /// Create a new instance of <see cref="AbstractUpdateContainer{T}"/>
    /// </summary>
    /// <param name="insiderResolver"></param>
    /// <param name="updater"></param>
    /// <param name="insider"></param>
    /// <param name="extraObjects"></param>
    protected AbstractUpdateContainer(
        Func<Update, T?> insiderResolver,
        IUpdater updater,
        Update insider,
        IReadOnlyDictionary<string, object>? extraObjects = default)
    {
        ShiningInfo = null!;
        Updater = updater;
        Container = insider;
        BotClient = updater.BotClient;
        _insiderResolver = insiderResolver;
        _extraObjects = extraObjects ?? new Dictionary<string, object>(StringComparer.Ordinal);
    }

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
    public virtual Update Container { get; }

    /// <inheritdoc/>
    public virtual ITelegramBotClient BotClient { get; }

    /// <inheritdoc/>
    public virtual ShiningInfo<long, Update> ShiningInfo { get; }

    /// <inheritdoc/>
    public virtual IUpdater Updater { get; }

    /// <inheritdoc/>
    public bool ContainsKey(string key) => _extraObjects.ContainsKey(key);
}
