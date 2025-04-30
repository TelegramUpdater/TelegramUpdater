using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// This class provides useful data for handlers.
/// </summary>
/// <typeparam name="TUpdate">
/// The type of inner actual update. One of <see cref="Update"/> properties.
/// </typeparam>
public abstract class AbstractHandlerProvider<TUpdate>
    : IHandlerProvider<TUpdate> where TUpdate : class
{
    /// <inheritdoc />
    public abstract IContainer<TUpdate> Container { get; protected set; }

    /// <summary>
    /// The updater instance.
    /// </summary>
    protected IUpdater Updater => Container.Updater;

    /// <summary>
    /// The update instance.
    /// </summary>
    protected Update Update => Container.Container;

    /// <summary>
    /// The actual update. one of <see cref="Update"/> properties.
    /// </summary>
    protected TUpdate ActualUpdate => Container.Update;

    /// <summary>
    /// Bot client instance.
    /// </summary>
    public ITelegramBotClient BotClient => Container.BotClient;

    #region Enum State
    /// <inheritdoc cref="Extensions.GetUserEnumStateKeeper{TEnum}(IUpdater, string)"/>
    public UserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>(string name)
        where TEnum : struct, Enum
        => Updater.GetUserEnumStateKeeper<TEnum>(name);

    /// <inheritdoc cref="Extensions.GetUserEnumStateKeeper{TEnum}(IUpdater)"/>
    public UserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>()
        where TEnum : struct, Enum
        => Updater.GetUserEnumStateKeeper<TEnum>();

    /// <inheritdoc cref="Extensions.TryGetUserEnumStateKeeper{TEnum}(IUpdater, string, out UserEnumStateKeeper{TEnum}?)"/>
    public bool TryGetUserEnumStateKeeper<TEnum>(
        string name,
        [NotNullWhen(true)] out UserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => Updater.TryGetUserEnumStateKeeper(name, out stateKeeper);

    /// <inheritdoc cref="Extensions.TryGetUserEnumStateKeeper{TEnum}(IUpdater, out UserEnumStateKeeper{TEnum}?)"/>
    public bool TryGetUserEnumStateKeeper<TEnum>(
        [NotNullWhen(true)] out UserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => Updater.TryGetUserEnumStateKeeper(out stateKeeper);

    /// <summary>
    /// Initialize the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool InitiateState<TEnum>(User user) where TEnum : struct, Enum
    {
        if (TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
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
    /// <param name="user"></param>
    /// <returns></returns>
    public bool ForwardState<TEnum>(User user) where TEnum : struct, Enum
    {
        if (TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            return stateKeeper.TryMoveForward(user, out var _);
        }

        return false;
    }

    /// <summary>
    /// Move the state of the user backward.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool BackwardState<TEnum>(User user) where TEnum : struct, Enum
    {
        if (TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            return stateKeeper.TryMoveBackward(user, out var _);
        }

        return false;
    }

    /// <summary>
    /// Set the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool SetState<TEnum>(User user, TEnum state) where TEnum : struct, Enum
    {
        if (TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
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
    /// <param name="user"></param>
    /// <returns></returns>
    public bool DeleteState<TEnum>(User user) where TEnum : struct, Enum
    {
        if (TryGetUserEnumStateKeeper<TEnum>(out var stateKeeper))
        {
            stateKeeper.DeleteState(user);
            return true;
        }
        return false;
    }

    #endregion

    /// <inheritdoc cref="Extensions.GetUserNumericStateKeeper(IUpdater, string)"/>
    public UserNumericStateKeeper GetUserNumericStateKeeper(string name)
        => Updater.GetUserNumericStateKeeper(name);
}
