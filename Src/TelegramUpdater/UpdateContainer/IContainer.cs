using Microsoft.Extensions.Primitives;
using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// Base class for update containers. ( Used while handling updates )
/// </summary>
public interface IContainer
{
    /// <summary>
    /// Raw inputs of the handler.
    /// </summary>
    public HandlerInput Input { get; }

    /// <summary>
    /// <see cref="TelegramUpdater.Updater"/> instance which is responsible.
    /// for this container.
    /// </summary>
    public IUpdater Updater => Input.Updater;

    /// <summary>
    /// The received update.
    /// </summary>
    public Update Container => ShiningInfo.Value;

    /// <summary>
    /// Processing info for this update.
    /// </summary>
    public ShiningInfo<long, Update> ShiningInfo => Input.ShiningInfo;

    /// <summary>
    /// <see cref="ITelegramBotClient"/> which is responsible for this container.
    /// </summary>
    public ITelegramBotClient BotClient => Updater.BotClient;

    internal IChangeToken? ScopeChangeToken => Input.ScopeChangeToken;

    internal IChangeToken? LayerChangeToken => Input.LayerChangeToken;

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

/// <summary>
/// A sub interface of <see cref="IContainer"/>, made for simplicity.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IContainer<T> : IContainer where T : class
{
    /// <summary>
    /// The actual update.
    /// </summary>
    public T Update { get; }
}
