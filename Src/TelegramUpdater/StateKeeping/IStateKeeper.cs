using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping;

public interface IStateKeeper<TState, TFrom> where TState : IEquatable<TState>
{
    /// <summary>
    /// Check if <paramref name="stateOf"/> has any <typeparamref name="TState"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <returns></returns>
    public bool HasAnyState(TFrom stateOf);

    /// <summary>
    /// Get the state for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <returns></returns>
    public TState GetState(TFrom stateOf);

    /// <summary>
    /// Tries to get the state for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="theState">State of <paramref name="stateOf"/></param>
    /// <returns></returns>
    public bool TryGetState(TFrom stateOf, [NotNullWhen(true)] out TState? theState);

    /// <summary>
    /// Set <paramref name="theState"/> for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="theState">State of <paramref name="stateOf"/></param>
    public void SetState(TFrom stateOf, TState theState);

    /// <summary>
    /// Checks if <paramref name="stateOf"/> has state <paramref name="theState"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>
    /// <param name="theState">State of <paramref name="stateOf"/></param>
    /// <returns></returns>
    public bool HasState(TFrom stateOf, TState theState);

    /// <summary>
    /// Delete the whole state of <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// Container object to get a unique <see cref="long"/> key from.
    /// </param>    
    /// <returns></returns>
    public bool DeleteState(TFrom stateOf);
}
