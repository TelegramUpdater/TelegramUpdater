using TelegramUpdater.Filters;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Creates an <see cref="IScopedUpdateHandler"/> for any type of update.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
/// <remarks>
/// Create a new instance of <see cref="AnyHandler{T}"/>.
/// </remarks>
/// <param name="getT">To extract <typeparamref name="T"/> from <see cref="Update"/>.</param>
/// <param name="group">Handling priority.</param>
public abstract class AnyHandler<T>(Func<Update, T?> getT)
    : AbstractScopedUpdateHandler<T>(getT)
    where T : class
{
    /// <inheritdoc/>
    internal protected sealed override IContainer<T> ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo)
    {
        return new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);
    }

    #region Extension Methods
    /// <summary>
    /// All pending handlers for this update will be ignored after throwing this.
    /// </summary>
    protected void StopPropagation() => Container.StopPropagation();

    /// <summary>
    /// Continue to the next pending handler for this update and ignore the rest of this handler.
    /// </summary>
    protected void ContinuePropagation() => Container.ContinuePropagation();

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
