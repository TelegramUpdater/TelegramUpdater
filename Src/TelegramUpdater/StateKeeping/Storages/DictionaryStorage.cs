using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping.Storages;

/// <summary>
/// <see cref="Dictionary{TKey, TValue}"/> storage for state keepers.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TState"></typeparam>
public sealed class DictionaryStorage<TKey, TState>
    : AbstractStateKeeperStorage<TKey, TState>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TState> _storage = [];

    /// <inheritdoc/>
    public override void Create(TKey key, TState state)
    {
        _storage.Add(key, state);
    }

    /// <inheritdoc/>
    public override void Delete(TKey key)
    {
        _storage.Remove(key);
    }

    /// <inheritdoc/>
    public override bool Exists(TKey key) => _storage.ContainsKey(key);

    /// <inheritdoc/>
    public override TState? Read(TKey key) => _storage[key];

    /// <inheritdoc/>
    public override bool TryGetValue(TKey key, [NotNullWhen(true)] out TState? value)
        => _storage.TryGetValue(key, out value);

    /// <inheritdoc/>
    public override void Update(TKey key, TState newState)
    {
        _storage[key] = newState;
    }
}
