using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping.StateKeepers;

/// <summary>
/// A numeric state keeper that increases and decreases an <see cref="int"/> state.
/// </summary>
/// <typeparam name="TKey">
/// The key.
/// </typeparam>
/// <typeparam name="TFrom">
/// The master object to extract key from.
/// </typeparam>
public abstract class AbstractNumericStateKeeper<TKey, TFrom>
    : AbstractStateKeeper<TKey, int, TFrom>
    where TKey : notnull
{
    /// <summary>
    /// Defines the acceptable range of state.
    /// </summary>
    /// <remarks>
    /// Use null to make the range unbound.
    /// </remarks>
    public virtual Range? StateRange { get; } = null;

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

            // TODO: CheckStateRangeValidity runs twice: here and inside SetState.
            var rangeValidity = CheckStateRangeValidity(ref newState);

            if (rangeValidity)
            {
                SetState(stateOf, newState!.Value);
                return true;
            }

            return false;
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

            // TODO: CheckStateRangeValidity runs twice: here and inside SetState.
            var rangeValidity = CheckStateRangeValidity(ref newState);

            if (rangeValidity)
            {
                SetState(stateOf, newState!.Value);
                return true;
            }

            return false;
        }

        newState = default;
        return false;
    }

    private bool CheckStateRangeValidity([NotNullWhen(true)] ref int? newState)
    {
        if (newState is int state)
        {
            if (StateRange is Range range)
            {
                if (state < range.Start.Value || state > range.End.Value)
                {
                    // Invalid state range
                    newState = default;
                    return false;
                }

                // valid state range
                return true;
            }

            // unbound state range
            return true;
        }

        // null state is not allowed
        return false;
    }

    /// <inheritdoc />
    public override bool CheckStateValidity(int newState)
    {
        var value = new int?(newState);
        return CheckStateRangeValidity(ref value);
    }

    /// <summary>
    /// Set default state for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    public void InitializeState(TFrom stateOf)
    {
        SetState(stateOf, default);
    }
}
