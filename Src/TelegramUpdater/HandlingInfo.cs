namespace TelegramUpdater;

/// <summary>
/// The handler and some more info about it.
/// </summary>
/// <typeparam name="T">Type of the handler.</typeparam>
/// <param name="handler">The handler.</param>
/// <param name="options">Options about how a handler should be handled.</param>
public class HandlingInfo<T>(
    T handler,
    HandlingOptions? options = default)
{
    /// <summary>s
    /// The handler.
    /// </summary>
    public T Handler { get; } = handler;

    /// <summary>
    /// The info
    /// </summary>
    public HandlingOptions? Options { get; } = options;
}


