using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Exceptions;
using TelegramUpdater.StateKeeping;
using TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;
using TelegramUpdater.StateKeeping.Storages;

namespace TelegramUpdater;

/// <summary>
/// Extension methods for <see cref="IStateKeeper{TKey, TState, TStorage}"/>.
/// </summary>
public static class Extensions
{
    internal const string StateKeeperKeyPrefix = "StateKeeper_";

    internal const string DefaultStateKeeperName = "DefaultStateKeeper";

    /// <summary>
    /// Register an <see cref="IStateKeeper{TKey, TState, TStorage}"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <param name="stateKeeper">The state keeper to add.</param>
    /// <returns></returns>
    public static IUpdater AddStateKeeper<TKey, TState, TStorage>(
        this IUpdater updater,
        string name,
        IStateKeeper<TKey, TState, TStorage> stateKeeper)
        where TKey : notnull
        where TStorage : IStateKeeperStorage<TKey, TState>
    {
        var key = StateKeeperKeyPrefix + name;

        if (updater.ContainsKey(key))
            throw new InvalidOperationException("Duplicated state keeper name.");

        updater[key] = stateKeeper;
        return updater;
    }

    /// <summary>
    /// Get a <see cref="IStateKeeper{TKey, TState, TStorage}"/> that you register before using
    /// <see cref="AddStateKeeper{TKey, TState, TStorage}(IUpdater, string, IStateKeeper{TKey, TState, TStorage})"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="StateKeeperNotRegisteredException"></exception>
    public static IStateKeeper<TKey, TState, TStorage> GetStateKeeper<TKey, TState, TStorage>(
        this IUpdater updater, string name)
        where TKey : notnull
        where TStorage : IStateKeeperStorage<TKey, TState>
    {
        var key = StateKeeperKeyPrefix + name;

        if (updater.TryGetValue(key, out object? keeper))
        {
            return (IStateKeeper<TKey, TState, TStorage>)keeper;
        }

        throw new StateKeeperNotRegisteredException(name);
    }

    /// <summary>
    /// Tries to get a <see cref="IStateKeeper{TKey, TState, TStorage}"/> that you register before using
    /// <see cref="AddStateKeeper{TKey, TState, TStorage}(IUpdater, string, IStateKeeper{TKey, TState, TStorage})"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The state keeper.</param>
    /// <returns></returns>
    /// <exception cref="StateKeeperNotRegisteredException"></exception>
    public static bool TryGetStateKeeper<TKey, TState, TStorage>(
        this IUpdater updater, string name,
        [NotNullWhen(true)] out IStateKeeper<TKey, TState, TStorage>? stateKeeper)
        where TKey : notnull
        where TStorage : IStateKeeperStorage<TKey, TState>
    {
        var key = StateKeeperKeyPrefix + name;

        if (updater.TryGetValue(key, out var keeper))
        {
            stateKeeper = (IStateKeeper<TKey, TState, TStorage>)keeper;
            return true;
        }

        stateKeeper = default;
        return false;
    }

    /// <summary>
    /// Register a <see cref="MemoryUserNumericStateKeeper"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <returns></returns>
    public static IUpdater AddUserNumericStateKeeper(
        this IUpdater updater, string name)
        => updater.AddStateKeeper(name, new MemoryUserNumericStateKeeper());

    /// <summary>
    /// Get a <see cref="MemoryUserNumericStateKeeper"/> that you register before using
    /// <see cref="AddUserNumericStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static MemoryUserNumericStateKeeper GetUserNumericStateKeeper(
         this IUpdater updater, string name)
        => (MemoryUserNumericStateKeeper)updater.GetStateKeeper<long, int, MemoryCacheStorage<long, int>>(name);

    /// <summary>
    /// Tries to get a <see cref="MemoryUserNumericStateKeeper"/> that you register before using
    /// <see cref="AddUserNumericStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The numeric state keeper.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static bool TryGetUserNumericStateKeeper(
         this IUpdater updater, string name,
         [NotNullWhen(true)] out MemoryUserNumericStateKeeper? stateKeeper)
    {
        if (TryGetStateKeeper<long, int, MemoryCacheStorage<long, int>>(updater, name, out var keeper))
        {
            stateKeeper = (MemoryUserNumericStateKeeper)keeper;
            return true;
        }

        stateKeeper = default;
        return false;
    }

    /// <summary>
    /// Register a <see cref="MemoryUserEnumStateKeeper{TEnum}"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">A name for state keeper.</param>
    /// <returns></returns>
    public static IUpdater AddUserEnumStateKeeper<TEnum>(
        this IUpdater updater, string name)
        where TEnum : struct, Enum => updater.AddStateKeeper(name, new MemoryUserEnumStateKeeper<TEnum>());

    internal static string DefaultEnumStateKeeperName<TEnum>() where TEnum : struct, Enum
        => DefaultStateKeeperName + "Enum" + typeof(TEnum).Name;

    /// <summary>
    /// Register a default <see cref="MemoryUserEnumStateKeeper{TEnum}"/> on <see cref="IUpdater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <returns></returns>
    public static IUpdater AddUserEnumStateKeeper<TEnum>(
        this IUpdater updater)
        where TEnum : struct, Enum
        => updater.AddUserEnumStateKeeper<TEnum>(DefaultEnumStateKeeperName<TEnum>());

    /// <summary>
    /// Get a <see cref="MemoryUserEnumStateKeeper{TEnum}"/> that you register before using
    /// <see cref="AddUserEnumStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static MemoryUserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>(
        this IUpdater updater, string name)
        where TEnum : struct, Enum
    {
        return (MemoryUserEnumStateKeeper<TEnum>)updater.GetStateKeeper<long, TEnum, MemoryCacheStorage<long, TEnum>>(name);
    }

    /// <summary>
    /// Get a default <see cref="MemoryUserEnumStateKeeper{TEnum}"/> that you register before using
    /// <see cref="AddUserEnumStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static MemoryUserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>(
        this IUpdater updater)
        where TEnum : struct, Enum
        => updater.GetUserEnumStateKeeper<TEnum>(DefaultEnumStateKeeperName<TEnum>());

    /// <summary>
    /// Tries to get a <see cref="MemoryUserEnumStateKeeper{TEnum}"/> that you register before using
    /// <see cref="AddUserEnumStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="name">The name of state keeper.</param>
    /// <param name="stateKeeper">The enum state keeper.</param>
    /// <returns></returns>
    public static bool TryGetUserEnumStateKeeper<TEnum>(
         this IUpdater updater, string name,
         [NotNullWhen(true)] out MemoryUserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
    {
        if (TryGetStateKeeper<long, TEnum, MemoryCacheStorage<long, TEnum>>(updater, name, out var keeper))
        {
            stateKeeper = (MemoryUserEnumStateKeeper<TEnum>)keeper;
            return true;
        }

        stateKeeper = default;
        return false;
    }

    /// <summary>
    /// Tries to get a default <see cref="MemoryUserEnumStateKeeper{TEnum}"/> that you register before using
    /// <see cref="AddUserEnumStateKeeper(IUpdater, string)"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="stateKeeper">The enum state keeper.</param>
    /// <returns></returns>
    public static bool TryGetUserEnumStateKeeper<TEnum>(
        this IUpdater updater,
        [NotNullWhen(true)] out MemoryUserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => TryGetUserEnumStateKeeper(updater, DefaultEnumStateKeeperName<TEnum>(), out stateKeeper);
}
