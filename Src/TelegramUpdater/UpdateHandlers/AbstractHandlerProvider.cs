using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Filters;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;
using TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;
using TelegramUpdater.UpdateChannels;
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
    public MemoryUserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>(string name)
        where TEnum : struct, Enum
        => Updater.GetUserEnumStateKeeper<TEnum>(name);

    /// <inheritdoc cref="Extensions.GetUserEnumStateKeeper{TEnum}(IUpdater)"/>
    public MemoryUserEnumStateKeeper<TEnum> GetUserEnumStateKeeper<TEnum>()
        where TEnum : struct, Enum
        => Container.GetUserEnumStateKeeper<TEnum>();

    /// <inheritdoc cref="Extensions.TryGetUserEnumStateKeeper{TEnum}(IUpdater, string, out MemoryUserEnumStateKeeper{TEnum}?)"/>
    public bool TryGetUserEnumStateKeeper<TEnum>(
        string name,
        [NotNullWhen(true)] out MemoryUserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => Container.TryGetUserEnumStateKeeper(name, out stateKeeper);

    /// <inheritdoc cref="Extensions.TryGetUserEnumStateKeeper{TEnum}(IUpdater, out MemoryUserEnumStateKeeper{TEnum}?)"/>
    public bool TryGetUserEnumStateKeeper<TEnum>(
        [NotNullWhen(true)] out MemoryUserEnumStateKeeper<TEnum>? stateKeeper)
        where TEnum : struct, Enum
        => Container.TryGetUserEnumStateKeeper(out stateKeeper);

    /// <summary>
    /// Initialize the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool InitiateState<TEnum>(User user) where TEnum : struct, Enum
        => Container.InitiateState<TEnum>(user);


    /// <summary>
    /// Move the state of the user forward.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool ForwardState<TEnum>(User user) where TEnum : struct, Enum
        => Container.ForwardState<TEnum>(user);


    /// <summary>
    /// Move the state of the user backward.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool BackwardState<TEnum>(User user) where TEnum : struct, Enum
        => Container.BackwardState<TEnum>(user);

    /// <summary>
    /// Set the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool SetState<TEnum>(User user, TEnum state) where TEnum : struct, Enum
        => Container.SetState<TEnum>(user, state);

    /// <summary>
    /// Delete the state of the user.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="user"></param>
    /// <returns></returns>
    public bool DeleteState<TEnum>(User user) where TEnum : struct, Enum => Container.DeleteState<TEnum>(user);

    #endregion

    /// <inheritdoc cref="Extensions.GetUserNumericStateKeeper(IUpdater, string)"/>
    public MemoryUserNumericStateKeeper GetUserNumericStateKeeper(string name)
        => Container.GetUserNumericStateKeeper(name);

    /// <summary>
    /// Stops the updater from handling other handlers after this.
    /// </summary>
    /// <exception cref="StopPropagationException"></exception>
    public void StopPropagation()
    {
        throw new StopPropagationException();
    }

    /// <summary>
    /// Stops the updater from handling this handler and jump to other handlers after this.
    /// </summary>
    /// <exception cref="ContinuePropagationException"></exception>
    public void ContinuePropagation()
    {
        throw new ContinuePropagationException();
    }

    #region Channels
    /// <inheritdoc cref="ChannelsExtensions.OpenChannel{TExp, TCur}(IContainer{TCur}, IGenericUpdateChannel{TExp}, Func{IUpdater, ShiningInfo{long, Update}, Task}?, CancellationToken)"/>
    public async ValueTask<IContainer<TExp>?> OpenChannel<TExp>(
        IGenericUpdateChannel<TExp> updateChannel,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
        where TExp : class
    {
        return await Container.OpenChannel(
            updateChannel, onUnrelatedUpdate, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Await for an specified <see cref="Message"/> update.
    /// </summary>
    /// <param name="timeOut">Maximum allowed time to wait for the update.
    /// </param>
    /// <param name="filter">Filter updates to get the right one.</param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public async ValueTask<IContainer<Message>?> AwaitMessage(
        Filter<UpdaterFilterInputs<Message>>? filter,
        TimeSpan? timeOut,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        return await Container.ChannelMessage(
            filter, timeOut, onUnrelatedUpdate, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Await for a click on inline keyboard buttons from current user.
    /// </summary>
    /// <param name="timeOut">Maximum allowed time to wait for the update.
    /// </param>
    /// <param name="callbackQueryRegex">
    /// A regex filter on <see cref="CallbackQuery"/>.
    /// </param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public async Task<IContainer<CallbackQuery>?> AwaitButtonClick(
        TimeSpan timeOut,
        CallbackQueryRegex callbackQueryRegex,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        return await Container.ChannelButtonClick(
            timeOut, callbackQueryRegex, onUnrelatedUpdate, cancellationToken).ConfigureAwait(false);
    }
    #endregion
}
