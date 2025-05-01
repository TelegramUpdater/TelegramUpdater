using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping;

/// <summary>
/// Storage for state keepers.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TState"></typeparam>
public abstract class AbstractStateKeeperStorage<TKey, TState>
    : IStateKeeperStorage<TKey, TState>
    where TKey : notnull
{
    /// <inheritdoc/>
    public abstract void Create(TKey key, TState state);

    /// <inheritdoc/>
    public abstract void Delete(TKey key);

    /// <inheritdoc/>
    public abstract bool Exists(TKey key);

    /// <inheritdoc/>
    public abstract TState? Read(TKey key);

    /// <inheritdoc/>
    public abstract void Update(TKey key, TState newState);

    /// <summary>
    /// <see cref="Create(TKey, TState)"/> if not exists, <see cref="Update(TKey, TState)"/> if exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newState"></param>
    public void CreateOrUpdate(TKey key, TState newState)
    {
        if (Exists(key))
        {
            Update(key, newState);
        }
        else
        {
            Create(key, newState);
        }
    }

    /// <summary>
    /// Updates the state if the key exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newState"></param>
    public void TryUpdate(TKey key, TState newState)
    {
        if (Exists(key))
        {
            Update(key, newState);
        }
    }

    /// <inheritdoc/>
    public abstract bool TryGetValue(TKey key, [NotNullWhen(true)] out TState? value);
}
