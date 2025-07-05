using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of extension methods for all of containers.
/// </summary>
public static class DefaultContainerExtensions
{
    #region Enum State
    /// <inheritdoc cref="Extensions.GetUserEnumStateKeeper{TEnum}(IUpdater, string)"/>
    public static MemoryUserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum, T>(this IBaseContainer container, string name)
        where TEnum : struct, Enum
        => container.Updater.GetUserEnumStateKeeper<TEnum>(name);

    /// <inheritdoc cref="Extensions.GetUserEnumStateKeeper{TEnum}(IUpdater)"/>
    public static MemoryUserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>(this IBaseContainer container)
        where TEnum : struct, Enum
        => container.Updater.GetUserEnumStateKeeper<TEnum>();

    /// <inheritdoc cref="Extensions.TryGetUserEnumStateKeeper{TEnum}(IUpdater, string, out MemoryUserEnumStateKeeper{TEnum}?)"/>
    public static bool TryGetUserEnumStateKeeper<TEnum>(
        this IBaseContainer container,
        string name,
        [NotNullWhen(true)] out MemoryUserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => container.Updater.TryGetUserEnumStateKeeper(name, out stateKeeper);

    /// <inheritdoc cref="Extensions.TryGetUserEnumStateKeeper{TEnum}(IUpdater, out MemoryUserEnumStateKeeper{TEnum}?)"/>
    public static bool TryGetUserEnumStateKeeper<TEnum>(
        this IBaseContainer container,
        [NotNullWhen(true)] out MemoryUserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => container.Updater.TryGetUserEnumStateKeeper(out stateKeeper);

    /// <summary>
    /// Initialize the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="container"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool InitiateState<TEnum>(this IBaseContainer container, User user) where TEnum : struct, Enum
    {
        if (container.TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            stateKeeper.InitializeState(user);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Move the state of the user forward.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="container"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool ForwardState<TEnum>(this IBaseContainer container, User user) where TEnum : struct, Enum
    {
        if (container.TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            return stateKeeper.TryMoveForward(user, out var _);
        }

        return false;
    }

    /// <summary>
    /// Move the state of the user backward.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="container"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool BackwardState<TEnum>(this IBaseContainer container, User user) where TEnum : struct, Enum
    {
        if (container.TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            return stateKeeper.TryMoveBackward(user, out var _);
        }

        return false;
    }

    /// <summary>
    /// Set the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="container"></param>
    /// <param name="user"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static bool SetState<TEnum>(this IBaseContainer container, User user, TEnum state) where TEnum : struct, Enum
    {
        if (container.TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            stateKeeper.SetState(user, state);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Delete the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="container"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static bool DeleteState<TEnum>(this IBaseContainer container, User user) where TEnum : struct, Enum
    {
        if (container.TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            stateKeeper.DeleteState(user);
            return true;
        }
        return false;
    }

    #endregion

    /// <inheritdoc cref="Extensions.GetUserNumericStateKeeper(IUpdater, string)"/>
    public static MemoryUserNumericStateKeeper GetUserNumericStateKeeper(this IBaseContainer container, string name)
        => container.Updater.GetUserNumericStateKeeper(name);

}
