using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.StateKeeping.Storages;

namespace TelegramUpdater.StateKeeping;

/// <inheritdoc cref="IStateKeeper{TState, TKey}"/>
public abstract class AbstractStateKeeper<TKey, TState, TFrom>
    : IStateKeeper<TKey, TState>
    where TKey: notnull
{
    private readonly DictionaryStorage<TKey, TState> _storage = new();

    /// <summary>
    /// The storage used to fetch states.
    /// </summary>
    public DictionaryStorage<TKey, TState> Storage => _storage;

    /// <summary>
    /// A function to extract a unique <see cref="long"/> key from 
    /// container object <typeparamref name="TKey"/>.
    /// </summary>
    protected abstract Func<TFrom, TKey> KeyResolver { get; }

    /// <inheritdoc/>
    public abstract bool CheckStateValidity(TState newState);

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState>.HasAnyState(TKey stateOf) => Storage.Exists(stateOf);

    /// <inheritdoc/>
    TState? IStateKeeper<TKey, TState>.GetState(TKey stateOf) => Storage.Read(stateOf);

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState>.TryGetState(TKey stateOf, [NotNullWhen(true)] out TState? theState)
    {
        if ((this as IStateKeeper<TKey, TState>).HasAnyState(stateOf))
        {
            theState = (this as IStateKeeper<TKey, TState>).GetState(stateOf)!;
            return true;
        }

        theState = default;
        return false;
    }

    /// <inheritdoc/>
    void IStateKeeper<TKey, TState>.SetState(TKey stateOf, TState theState)
    {
        if (!CheckStateValidity(theState))
            return;

        if ((this as IStateKeeper<TKey, TState>).HasAnyState(stateOf))
            Storage.Update(stateOf, theState);
        else
            Storage.Create(stateOf, theState);
    }

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState>.HasState(TKey stateOf, TState theState)
    {
        if (!(this as IStateKeeper<TKey, TState>).HasAnyState(stateOf)) return false;
        return (this as IStateKeeper<TKey, TState>).GetState(stateOf)!.Equals(theState);
    }

    /// <inheritdoc/>
    bool IStateKeeper<TKey, TState>.DeleteState(TKey stateOf)
    {
        if ((this as IStateKeeper<TKey, TState>).HasAnyState(stateOf))
        {
            Storage.Delete(stateOf);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="IStateKeeper{TKey, TState}.HasAnyState(TKey)"/>
    public bool HasAnyState(TFrom stateOf) => (this as IStateKeeper<TKey, TState>).HasAnyState(KeyResolver(stateOf));

    /// <inheritdoc cref="IStateKeeper{TKey, TState}.GetState(TKey)"/>
    public TState? GetState(TFrom stateOf) => (this as IStateKeeper<TKey, TState>).GetState(KeyResolver(stateOf));

    /// <inheritdoc cref="IStateKeeper{TKey, TState}.TryGetState(TKey, out TState)" />
    public bool TryGetState(TFrom stateOf, [NotNullWhen(true)] out TState? theState)
        => (this as IStateKeeper<TKey, TState>).TryGetState(KeyResolver(stateOf), out theState);

    /// <inheritdoc cref="IStateKeeper{TKey, TState}.SetState(TKey, TState)"/>
    public void SetState(TFrom stateOf, TState theState) => (this as IStateKeeper<TKey, TState>).SetState(KeyResolver(stateOf), theState);

    /// <inheritdoc cref="IStateKeeper{TKey, TState}.HasState(TKey, TState)"/>
    public bool HasState(TFrom stateOf, TState theState) => (this as IStateKeeper<TKey, TState>).HasState(KeyResolver(stateOf), theState);

    /// <inheritdoc cref="IStateKeeper{TKey, TState}.DeleteState(TKey)"/>
    public bool DeleteState(TFrom stateOf) => (this as IStateKeeper<TKey, TState>).DeleteState(KeyResolver(stateOf));
}
