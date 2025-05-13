using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extensions for <see cref="UserNumericStateKeeper{TStorage}"/>
/// </summary>
public static class NumericStateKeeperExtensions
{
    /// <summary>
    /// Directly set an numeric state for <paramref name="user"/>.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="stateKeeperName">The state keeper name.</param>
    /// <param name="user">The user to set state for.</param>
    /// <param name="state">The state.</param>
    /// <returns></returns>
    public static bool SetNumericState<T>(
        this IBaseContainer container,
        string stateKeeperName,
        User user, int state = 0) where T : class
    {
        if (container.Updater.TryGetUserNumericStateKeeper(
            stateKeeperName, out var keeper))
        {
            keeper.SetState(user, state);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Directly forward an numeric state for <paramref name="user"/>.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="stateKeeperName">The state keeper name.</param>
    /// <param name="user">The user to set state for.</param>
    /// <param name="state">The state.</param>
    /// <returns></returns>
    public static bool TryForwardNumericState<T>(
        this IBaseContainer container,
        string stateKeeperName, User user,
        [NotNullWhen(true)] out int? state) where T : class
    {
        if (container.Updater.TryGetUserNumericStateKeeper(
            stateKeeperName, out var keeper))
        {
            return keeper.TryMoveForward(user, out state);
        }

        state = default;
        return false;
    }

    /// <summary>
    /// Directly backward an numeric state for <paramref name="user"/>.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="stateKeeperName">The state keeper name.</param>
    /// <param name="user">The user to set state for.</param>
    /// <param name="state">The state.</param>
    /// <returns></returns>
    public static bool TryBackwardNumericState<T>(
        this IBaseContainer container,
        string stateKeeperName, User user,
        [NotNullWhen(true)] out int? state) where T : class
    {
        if (container.Updater.TryGetUserNumericStateKeeper(
            stateKeeperName, out var keeper))
        {
            return keeper.TryMoveBackward(user, out state);
        }

        state = default;
        return false;
    }

    /// <summary>
    /// Directly delete an numeric state for <paramref name="user"/>.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="stateKeeperName">The state keeper name.</param>
    /// <param name="user">The user to set state for.</param>
    /// <returns></returns>
    public static bool DeleteNumericState<T>(
        this IBaseContainer container,
        string stateKeeperName, User user) where T : class
    {
        if (container.Updater.TryGetUserNumericStateKeeper(
            stateKeeperName, out var keeper))
        {
            return keeper.DeleteState(user);
        }

        return false;
    }
}
