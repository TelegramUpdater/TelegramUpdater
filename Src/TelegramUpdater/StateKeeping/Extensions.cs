using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Exceptions;
using TelegramUpdater.StateKeeping;
using TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

namespace TelegramUpdater;

/// <summary>
/// Extension methods for <see cref="IStateKeeper{TKey, TState}"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Register an <see cref="IStateKeeper{TKey, TState}"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <param name="stateKeeper">The state keeper to add.</param>
    /// <returns></returns>
    public static IUpdater AddStateKeeper<TKey, TState>(
        this IUpdater updater, string name, IStateKeeper<TKey, TState> stateKeeper)
        where TKey : notnull
    {
        var key = "StateKeeper_" + name;

        if (updater.ContainsKey(key))
            throw new InvalidOperationException("Duplicated state keeper name.");

        updater[key] = stateKeeper;
        return updater;
    }

    /// <summary>
    /// Get a <see cref="IStateKeeper{TKey, TState}"/> that you register before using
    /// <see cref="AddStateKeeper{TKey, TState}(IUpdater, string, IStateKeeper{TKey, TState})"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="StateKeeperNotRegistried"></exception>
    public static IStateKeeper<TKey, TState> GetStateKeeper<TKey, TState>(
        this IUpdater updater, string name)
        where TKey : notnull
    {
        var key = "StateKeeper_" + name;

        if (updater.ContainsKey(key))
        {
            return (IStateKeeper<TKey, TState>)updater[key];
        }

        throw new StateKeeperNotRegistried(name);
    }

    /// <summary>
    /// Tries to get a <see cref="IStateKeeper{TKey, TState}"/> that you register before using
    /// <see cref="AddStateKeeper{TKey, TState}(IUpdater, string, IStateKeeper{TKey, TState})"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The state keeper.</param>
    /// <returns></returns>
    /// <exception cref="StateKeeperNotRegistried"></exception>
    public static bool TryGetStateKeeper<TKey, TState>(
        this IUpdater updater, string name,
        [NotNullWhen(true)] out IStateKeeper<TKey, TState>? stateKeeper)
        where TKey : notnull
    {
        var key = "StateKeeper_" + name;

        if (updater.ContainsKey(key))
        {
            stateKeeper = (IStateKeeper<TKey, TState>)updater[key];
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
        return updater.AddStateKeeper<long, int>(name, new UserNumericStateKeeper());
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
        return (UserNumericStateKeeper)updater.GetStateKeeper<long, int>(name);
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
        if (TryGetStateKeeper<long, int>(updater, name, out var keeper))
        {
            stateKeeper = (UserNumericStateKeeper)keeper;
            return true;
        }

        stateKeeper = default;
        return false;
    }

    /// <summary>
    /// Register a <see cref="UserEnumStateKeeper{TEnum}"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <returns></returns>
    public static IUpdater AddUserEnumStateKeeper<TEnum>(
        this IUpdater updater, string name)
        where TEnum : struct, Enum
    {
        return updater.AddStateKeeper<long, TEnum>(name, new UserEnumStateKeeper<TEnum>());
    }

    /// <summary>
    /// Get a <see cref="UserEnumStateKeeper{TEnum}"/> that you register before using
    /// <see cref="AddUserEnumStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static UserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>(
        this IUpdater updater, string name)
        where TEnum : struct, Enum
    {
        return (UserEnumStateKeeper<TEnum>)updater.GetStateKeeper<long, TEnum>(name);
    }

    /// <summary>
    /// Tries to get a <see cref="UserEnumStateKeeper{TEnum}"/> that you register before using
    /// <see cref="AddUserEnumStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The enum state keeper.</param>
    /// <returns></returns>
    public static bool TryGetUserEnumStateKeeper<TEnum>(
         this IUpdater updater, string name,
         [NotNullWhen(true)] out UserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
    {
        if (TryGetStateKeeper<long, TEnum>(updater, name, out var keeper))
        {
            stateKeeper = (UserEnumStateKeeper<TEnum>)keeper;
            return true;
        }

        stateKeeper = default;
        return false;
    }
}
