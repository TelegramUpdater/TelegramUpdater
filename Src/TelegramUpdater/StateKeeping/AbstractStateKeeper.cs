using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping;

/// <inheritdoc cref="IStateKeeper{TState, TFrom}"/>
public abstract class AbstractStateKeeper<TState, TFrom> : IStateKeeper<TState, TFrom>
{
    private readonly Dictionary<long, TState> _state;

    /// <summary>
    /// Initialize <see cref="AbstractStateKeeper{TState, TFrom}"/>
    /// </summary>
    protected AbstractStateKeeper()
    {
        _state = [];
    }

    /// <summary>
    /// A function to extract a unique <see cref="long"/> key from 
    /// container object <typeparamref name="TFrom"/>.
    /// </summary>
    protected abstract Func<TFrom, long> KeyResolver { get; }

    /// <inheritdoc/>
    public abstract bool CheckStateValidity(TState newState);

    /// <inheritdoc/>
    public bool HasAnyState(TFrom stateOf) => _state.ContainsKey(KeyResolver(stateOf));

    /// <inheritdoc/>
    public TState GetState(TFrom stateOf) => _state[KeyResolver(stateOf)];

    /// <inheritdoc/>
    public bool TryGetState(TFrom stateOf, [NotNullWhen(true)] out TState? theState)
    {
        if (HasAnyState(stateOf))
        {
            theState = GetState(stateOf)!;
            return true;
        }

        theState = default;
        return false;
    }

    /// <inheritdoc/>
    public void SetState(TFrom stateOf, TState theState)
    {
        if (!CheckStateValidity(theState))
            return;

        if (HasAnyState(stateOf))
            _state[KeyResolver(stateOf)] = theState;
        else
            _state.Add(KeyResolver(stateOf), theState);
    }

    /// <inheritdoc/>
    public bool HasState(TFrom stateOf, TState theState)
    {
        if (!HasAnyState(stateOf)) return false;
        return GetState(stateOf)!.Equals(theState);
    }

    /// <inheritdoc/>
    public bool DeleteState(TFrom stateOf)
    {
        if (HasAnyState(stateOf))
        {
            return _state.Remove(KeyResolver(stateOf));
        }

        return false;
    }
}
