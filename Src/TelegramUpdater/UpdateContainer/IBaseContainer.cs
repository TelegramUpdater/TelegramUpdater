namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// Basic update container. Only includes <see cref="Update"/> and <see cref="IUpdater"/>.
/// </summary>
public interface IBaseContainer
{
    /// <summary>
    /// <see cref="IUpdater"/> instance which is responsible.
    /// for this container.
    /// </summary>
    public IUpdater Updater { get; }

    /// <summary>
    /// The received update.
    /// </summary>
    public Update Container { get; }

    /// <summary>
    /// <see cref="ITelegramBotClient"/> which is responsible for this container.
    /// </summary>
    public ITelegramBotClient BotClient => Updater.BotClient;
}

/// <summary>
/// A sub interface of <see cref="IBaseContainer"/>, made for simplicity.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseContainer<T> : IBaseContainer where T : class
{
    /// <summary>
    /// The actual update.
    /// </summary>
    public T Update { get; }
}