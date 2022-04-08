using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Exceptions;
using TelegramUpdater.StateKeeping;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

namespace TelegramUpdater;

/// <summary>
/// Extension methods for <see cref="IStateKeeper{TState, TFrom}"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Register an <see cref="IStateKeeper{TState, TFrom}"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <param name="stateKeeper">The state keeper to add.</param>
    /// <returns></returns>
    public static IUpdater AddStateKeeper<TState, TFrom>(
        this IUpdater updater, string name, IStateKeeper<TState, TFrom> stateKeeper)
        where TState : IEquatable<TState>
    {
        var key = "StateKeeper_" + name;

        if (updater.ContainsKey(key))
            throw new InvalidOperationException("Duplicated state keeper name.");

        updater[key] = stateKeeper;
        return updater;
    }

    /// <summary>
    /// Get a <see cref="IStateKeeper{TState, TFrom}"/> that you register before using
    /// <see cref="AddStateKeeper{TState, TFrom}(IUpdater, string, IStateKeeper{TState, TFrom})"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="StateKeeperNotRegistried"></exception>
    public static IStateKeeper<TState, TFrom> GetStateKeeper<TState, TFrom>(
        this IUpdater updater, string name)
        where TState : IEquatable<TState>
    {
        var key = "StateKeeper_" + name;

        if (updater.ContainsKey(key))
        {
            return (IStateKeeper<TState, TFrom>)updater[key];
        }

        throw new StateKeeperNotRegistried(name);
    }

    /// <summary>
    /// Tries to get a <see cref="IStateKeeper{TState, TFrom}"/> that you register before using
    /// <see cref="AddStateKeeper{TState, TFrom}(IUpdater, string, IStateKeeper{TState, TFrom})"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The state keeper.</param>
    /// <returns></returns>
    /// <exception cref="StateKeeperNotRegistried"></exception>
    public static bool TryGetStateKeeper<TState, TFrom>(
        this IUpdater updater, string name,
        [NotNullWhen(true)] out IStateKeeper<TState, TFrom>? stateKeeper)
        where TState : IEquatable<TState>
    {
        var key = "StateKeeper_" + name;

        if (updater.ContainsKey(key))
        {
            stateKeeper = (IStateKeeper<TState, TFrom>)updater[key];
            return true;
        }

        stateKeeper = default;
        return false;
    }

    /// <summary>
    /// Register a <see cref="UserNumericStateKeeper"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <returns></returns>
    public static IUpdater AddUserNumericStateKeeper(
        this IUpdater updater, string name)
    {
        return updater.AddStateKeeper(name, new UserNumericStateKeeper());
    }

    /// <summary>
    /// Get a <see cref="UserNumericStateKeeper"/> that you register before using
    /// <see cref="AddUserNumericStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static UserNumericStateKeeper GetUserNumericStateKeeper(
         this IUpdater updater, string name)
    {
        return (UserNumericStateKeeper)updater.GetStateKeeper<int, User>(name);
    }

    /// <summary>
    /// Tries to get a <see cref="UserNumericStateKeeper"/> that you register before using
    /// <see cref="AddUserNumericStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The numeric state keeper.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static bool TryGetUserNumericStateKeeper(
         this IUpdater updater, string name,
         [NotNullWhen(true)] out UserNumericStateKeeper? stateKeeper)
    {
        if (TryGetStateKeeper<int, User>(updater, name, out var keeper))
        {
            stateKeeper = (UserNumericStateKeeper)keeper;
            return true;
        }

        stateKeeper = default;
        return false;
    }
}
