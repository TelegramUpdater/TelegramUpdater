using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping.Storages;

/// <summary>
/// Uses <see cref="Microsoft.Extensions.Caching.Memory.MemoryCache"/> as storage.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TState"></typeparam>
public class MemoryCacheStorage<TKey, TState>
    : AbstractStateKeeperStorage<TKey, TState> where TKey : notnull
{
    /// <summary>
    /// The inner <see cref="Microsoft.Extensions.Caching.Memory.MemoryCache"/>.
    /// </summary>
    public MemoryCache MemoryCache { get; } = new(new MemoryCacheOptions());

    /// <inheritdoc/>
    public override void Create(TKey key, TState state)
    {
        MemoryCache.Set(key, state);
    }

    /// <inheritdoc/>
    public override void Delete(TKey key)
    {
        MemoryCache.Remove(key);
    }

    /// <inheritdoc/>
    public override bool Exists(TKey key) => MemoryCache.TryGetValue(key, out var _);

    /// <inheritdoc/>
    public override TState? Read(TKey key) => (TState?)MemoryCache.Get(key);

    /// <inheritdoc/>
    public override bool TryGetValue(TKey key, [NotNullWhen(true)] out TState? value)
        => MemoryCache.TryGetValue(key, out value);

    /// <inheritdoc/>
    public override void Update(TKey key, TState newState)
    {
        MemoryCache.Set(key, newState);
    }
}
