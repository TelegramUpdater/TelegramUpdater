using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping;

public abstract class AbstractStateKeeper<TState, TFrom> : IStateKeeper<TState, TFrom>
    where TState : IEquatable<TState>
{
    private readonly Dictionary<long, TState> _state;

    protected AbstractStateKeeper()
    {
        _state = new();
    }

    /// <summary>
    /// A function to extract a unique <see cref="long"/> key from 
    /// container object <typeparamref name="TFrom"/>.
    /// </summary>
    protected abstract Func<TFrom, long> KeyResolver { get; }

    /// <inheritdoc/>
    public bool HasAnyState(TFrom stateOf) => _state.ContainsKey(KeyResolver(stateOf));

    /// <inheritdoc/>
    public TState GetState(TFrom stateOf) => _state[KeyResolver(stateOf)];

    /// <inheritdoc/>
    public bool TryGetState(TFrom stateOf, [NotNullWhen(true)] out TState? theState)
    {
        if (HasAnyState(stateOf))
        {
            theState = _state[KeyResolver(stateOf)];
            return true;
        }

        theState = default;
        return false;
    }

    /// <inheritdoc/>
    public void SetState(TFrom stateOf, TState theState)
    {
        if (HasAnyState(stateOf))
            _state[KeyResolver(stateOf)] = theState;
        else
            _state.Add(KeyResolver(stateOf), theState);
    }

    /// <inheritdoc/>
    public bool HasState(TFrom stateOf, TState theState)
    {
        if (!HasAnyState(stateOf)) return false;
        return _state[KeyResolver(stateOf)].Equals(theState);
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
