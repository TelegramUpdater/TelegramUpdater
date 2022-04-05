using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping.StateKeepers;

/// <summary>
/// A numeric state keeper that increases and decreases an <see cref="int"/> state.
/// </summary>
/// <typeparam name="TFrom">
/// A container object that is used to extract an unique <see cref="long"/> from.
/// </typeparam>
public abstract class AbstractNumericStateKeeper<TFrom> : AbstractStateKeeper<int, TFrom>
{
    /// <summary>
    /// Tries to increase state by one.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="newState">The new state of <paramref name="stateOf"/>.</param>
    /// <returns></returns>
    public bool TryMoveForward(TFrom stateOf, [NotNullWhen(true)] out int? newState)
    {
        if (HasAnyState(stateOf))
        {
            var state = GetState(stateOf);
            newState = state + 1;
            SetState(stateOf, newState.Value);
            return true;
        }

        newState = default;
        return false;
    }

    /// <summary>
    /// Tries to decrease state by one.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="newState">The new state of <paramref name="stateOf"/>.</param>
    /// <returns></returns>
    public bool TryMoveBackward(TFrom stateOf, [NotNullWhen(true)] out int? newState)
    {
        if (HasAnyState(stateOf))
        {
            var state = GetState(stateOf);
            newState = state - 1;
            SetState(stateOf, newState.Value);
            return true;
        }

        newState = default;
        return false;
    }
}
