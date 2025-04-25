namespace TelegramUpdater.StateKeeping;

/// <summary>
/// Storage for state keepers.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TState"></typeparam>
public interface IStateKeeperStorage<TKey, TState>
    where TKey: notnull
{
    /// <summary>
    /// Creates a new state for <paramref name="key"/> if not exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="state"></param>
    void Create(TKey key, TState state);

    /// <summary>
    /// Reads the state of <paramref name="key"/> if exists.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    TState? Read(TKey key);

    /// <summary>
    /// Update the state to a new one if possible.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newState"></param>
    void Update(TKey key, TState newState);

    /// <summary>
    /// Deletes the state for <paramref name="key"/>
    /// </summary>
    /// <param name="key"></param>
    void Delete(TKey key);

    /// <summary>
    /// Checks if a <paramref name="key"/> has any state.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Exists(TKey key);
}
