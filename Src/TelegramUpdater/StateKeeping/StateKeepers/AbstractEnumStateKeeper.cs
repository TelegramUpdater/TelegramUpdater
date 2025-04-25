using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping.StateKeepers;

/// <summary>
/// An enum state than can move forward or backward between enum values.
/// </summary>
/// <remarks>
/// The propagation is based on enum values, definition order. NOT their numeric values.
/// </remarks>
/// <typeparam name="TEnum">The enum.</typeparam>
/// <typeparam name="TFrom">The master object to extract state key from.</typeparam>
public abstract class AbstractEnumStateKeeper<TEnum, TFrom> : AbstractStateKeeper<TEnum, TFrom>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Tries to move to the next enum state.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="newState">The new state of <paramref name="stateOf"/>.</param>
    /// <returns></returns>
    public bool TryMoveForward(TFrom stateOf, [NotNullWhen(true)] out TEnum? newState)
    {
        if (HasAnyState(stateOf))
        {
            var currentState = GetState(stateOf);
            var enumValues = Enum.GetValues<TEnum>();
            var currentIndex = Array.IndexOf(enumValues, currentState);
            var newIndex = currentIndex + 1;

            if (newIndex < enumValues.Length)
            {
                newState = enumValues[newIndex];
                SetState(stateOf, newState.Value);
                return true;
            }

            newState = default;
            return false;
        }

        newState = default;
        return false;
    }

    /// <summary>
    /// Tries to move to the previous enum state.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="newState">The new state of <paramref name="stateOf"/>.</param>
    /// <returns></returns>
    public bool TryMoveBackward(TFrom stateOf, [NotNullWhen(true)] out TEnum? newState)
    {
        if (HasAnyState(stateOf))
        {
            var currentState = GetState(stateOf);
            var enumValues = Enum.GetValues<TEnum>();
            var currentIndex = Array.IndexOf(enumValues, currentState);
            var newIndex = currentIndex - 1;

            if (newIndex >= 0)
            {
                newState = enumValues[newIndex];
                SetState(stateOf, newState.Value);
                return true;
            }

            newState = default;
            return false;
        }

        newState = default;
        return false;
    }

    /// <inheritdoc />
    public override bool CheckStateValidity(TEnum newState) => true;

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
