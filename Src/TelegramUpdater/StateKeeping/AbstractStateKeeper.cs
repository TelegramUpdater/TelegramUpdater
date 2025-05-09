using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping;

/// <inheritdoc cref="IStateKeeper{TState, TKey, TStorage}"/>
/// <summary>
/// Create a new instance of this while setting the <typeparamref name="TStorage"/>.
/// </summary>
/// <param name="storage"></param>
public abstract class AbstractStateKeeper<TKey, TState, TFrom, TStorage>(TStorage storage)
    : IStateKeeper<TKey, TState, TStorage>
    where TKey: notnull
    where TStorage: IStateKeeperStorage<TKey, TState>
{
    /// <summary>
    /// A function to extract a unique <see cref="long"/> key from 
    /// container object <typeparamref name="TKey"/>.
    /// </summary>
    protected abstract Func<TFrom, TKey> KeyResolver { get; }

    /// <inheritdoc/>
    public TStorage Storage { get; } = storage;

    /// <inheritdoc/>
    public abstract bool CheckStateValidity(TState newState);

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState, TStorage>.HasAnyState(TKey stateOf) => Storage.Exists(stateOf);

    /// <inheritdoc/>
    TState? IStateKeeper<TKey, TState, TStorage>.GetState(TKey stateOf) => Storage.Read(stateOf);

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState, TStorage>.TryGetState(TKey stateOf, [NotNullWhen(true)] out TState? theState)
    {
        if ((this as IStateKeeper<TKey, TState, TStorage>).HasAnyState(stateOf))
        {
            theState = (this as IStateKeeper<TKey, TState, TStorage>).GetState(stateOf)!;
            return true;
        }

        theState = default;
        return false;
    }

    /// <inheritdoc/>
    void IStateKeeper<TKey, TState, TStorage>.SetState(TKey stateOf, TState theState)
    {
        if (!CheckStateValidity(theState))
            return;

        if ((this as IStateKeeper<TKey, TState, TStorage>).HasAnyState(stateOf))
            Storage.Update(stateOf, theState);
        else
            Storage.Create(stateOf, theState);
    }

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState, TStorage>.HasState(TKey stateOf, TState theState)
    {
        if (!(this as IStateKeeper<TKey, TState, TStorage>).HasAnyState(stateOf)) return false;
        return (this as IStateKeeper<TKey, TState, TStorage>).GetState(stateOf)!.Equals(theState);
    }

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState, TStorage>.DeleteState(TKey stateOf)
    {
        if ((this as IStateKeeper<TKey, TState, TStorage>).HasAnyState(stateOf))
        {
            Storage.Delete(stateOf);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="IStateKeeper{TKey, TState, TStorage}.HasAnyState(TKey)"/>
    public bool HasAnyState(TFrom stateOf) => (this as IStateKeeper<TKey, TState, TStorage>).HasAnyState(KeyResolver(stateOf));

    /// <inheritdoc cref="IStateKeeper{TKey, TState, TStorage}.GetState(TKey)"/>
    public TState? GetState(TFrom stateOf) => (this as IStateKeeper<TKey, TState, TStorage>).GetState(KeyResolver(stateOf));

    /// <inheritdoc cref="IStateKeeper{TKey, TState, TStorage}.TryGetState(TKey, out TState)" />
    public bool TryGetState(TFrom stateOf, [NotNullWhen(true)] out TState? theState)
        => (this as IStateKeeper<TKey, TState, TStorage>).TryGetState(KeyResolver(stateOf), out theState);

    /// <inheritdoc cref="IStateKeeper{TKey, TState, TStorage}.SetState(TKey, TState)"/>
    public void SetState(TFrom stateOf, TState theState) => (this as IStateKeeper<TKey, TState, TStorage>).SetState(KeyResolver(stateOf), theState);

    /// <inheritdoc cref="IStateKeeper{TKey, TState, TStorage}.HasState(TKey, TState)"/>
    public bool HasState(TFrom stateOf, TState theState) => (this as IStateKeeper<TKey, TState, TStorage>).HasState(KeyResolver(stateOf), theState);

    /// <inheritdoc cref="IStateKeeper{TKey, TState, TStorage}.DeleteState(TKey)"/>
    public bool DeleteState(TFrom stateOf) => (this as IStateKeeper<TKey, TState, TStorage>).DeleteState(KeyResolver(stateOf));
}
