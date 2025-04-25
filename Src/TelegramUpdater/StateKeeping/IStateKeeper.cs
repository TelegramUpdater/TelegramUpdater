using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.StateKeeping;

/// <summary>
/// Interface for state keepers.
/// </summary>
/// <typeparam name="TState">The state itself which must implement <see cref="IEquatable{TState}"/></typeparam>
/// <typeparam name="TKey">The master object which we extract the state key from</typeparam>
public interface IStateKeeper<TKey, TState> where TKey: notnull
{
    /// <summary>
    /// Check if <paramref name="stateOf"/> has any <typeparamref name="TState"/>.
    /// </summary>
    /// <param name="stateOf">
    /// The key to get state for.
    /// </param>
    /// <returns></returns>
    public bool HasAnyState(TKey stateOf);

    /// <summary>
    /// Get the state for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// The key to get state for.
    /// </param>
    /// <returns></returns>
    public TState? GetState(TKey stateOf);

    /// <summary>
    /// Tries to get the state for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// The key to get state for.
    /// </param>
    /// <param name="theState">State of <paramref name="stateOf"/></param>
    /// <returns></returns>
    public bool TryGetState(TKey stateOf, [NotNullWhen(true)] out TState? theState);

    /// <summary>
    /// Set <paramref name="theState"/> for <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// The key to get state for.
    /// </param>
    /// <param name="theState">State of <paramref name="stateOf"/></param>
    public void SetState(TKey stateOf, TState theState);

    /// <summary>
    /// Checks if <paramref name="stateOf"/> has state <paramref name="theState"/>.
    /// </summary>
    /// <param name="stateOf">
    /// The key to get state for.
    /// </param>
    /// <param name="theState">State of <paramref name="stateOf"/></param>
    /// <returns></returns>
    public bool HasState(TKey stateOf, TState theState);

    /// <summary>
    /// Delete the whole state of <paramref name="stateOf"/>.
    /// </summary>
    /// <param name="stateOf">
    /// The key to get state for.
    /// </param>    
    /// <returns></returns>
    public bool DeleteState(TKey stateOf);

    /// <summary>
    /// Checks if a new state is valid to be set.
    /// </summary>
    /// <param name="newState">The state to check against.</param>
    /// <returns></returns>
    public bool CheckStateValidity(TState newState);
}
