using TelegramUpdater.Filters;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateChannels.ReadyToUse;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// A set of extension methods for <see cref="IUpdateChannel"/>s.
/// </summary>
public static class ChannelsExtensions
{
    /// <summary>
    /// Open a channel an wait for an specified update from user.
    /// </summary>
    /// <typeparam name="TExp">Update type you're expecting.</typeparam>
    /// <typeparam name="TCur">Current container update type.</typeparam>
    /// <param name="container">The container itself.</param>
    /// <param name="updateChannel">
    /// Your channel to choose the right update.
    /// </param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public static async ValueTask<IContainer<TExp>?> OpenChannel<TExp, TCur>(
        this IContainer<TCur> container,
        IGenericUpdateChannel<TExp> updateChannel,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
        where TExp : class where TCur : class
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(container);
#else
        if (container is null)
            throw new ArgumentNullException(nameof(container));
#endif

        if (updateChannel == null)
        {
            throw new InvalidOperationException(
                "abstractChannel and updateResolver both can't be null");
        }

        // A secondary timeOut, because ReadNextAsync'timeout will reset on unrelated update.
        var timeOutCts = new CancellationTokenSource();
        timeOutCts.CancelAfter(updateChannel.TimeOut);

        using var linkedCts = CancellationTokenSource
            .CreateLinkedTokenSource(timeOutCts.Token, cancellationToken);

        while (true)
        {
            try
            {
                var update = await container.ShiningInfo.ReadNextAsync(
                    updateChannel.TimeOut, linkedCts.Token).ConfigureAwait(false);

                if (update == null)
                    return null;

                if (updateChannel.ShouldChannel(new(container.Updater, update.Value)))
                {
                    return new AnyContainer<TExp>(
                        updateChannel.GetActualUpdate,
                        container.Updater,
                        update,
                        updateChannel.ExtraData
                    );
                }

                if (onUnrelatedUpdate != null)
                {
                    await onUnrelatedUpdate(container.Updater, update).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                if (timeOutCts.IsCancellationRequested) return default;
                throw;
            }
        }
    }

    /// <summary>
    /// Opens a channel that dispatches a <see cref="Message"/> from updater.
    /// </summary>
    /// <param name="updateContainer">The update container</param>
    /// <param name="timeOut">Maximum allowed time to wait for the update.
    /// </param>
    /// <param name="filter">Filter updates to get the right one.</param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    public static async Task<IContainer<Message>?> ChannelMessage<K>(
        this IContainer<K> updateContainer,
        Filter<UpdaterFilterInputs<Message>>? filter,
        TimeSpan? timeOut,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default) where K : class
    {
        return await updateContainer.OpenChannel(
            new MessageChannel(timeOut ?? TimeSpan.FromSeconds(30), filter),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }


    /// <summary>
    /// Opens a channel that dispatches a <see cref="Message"/>
    /// of this user from updater.
    /// </summary>
    /// <param name="updateContainer">The update container</param>
    /// <param name="timeOut">Maximum allowed time to wait for the update.
    /// </param>
    /// <param name="filter">Filter updates to get the right one.</param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    [Obsolete("Use ChannelMessageAsync.")]
    public static async Task<IContainer<Message>?> ChannelUserResponse(
        this IContainer<Message> updateContainer,
        TimeSpan timeOut,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        Filter<UpdaterFilterInputs<Message>>? filter = default,
        CancellationToken cancellationToken = default)
    {
        return await updateContainer.ChannelMessage(
            filter, timeOut, onUnrelatedUpdate, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for the user to click on an inline button.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static async Task<IContainer<CallbackQuery>?> ChannelButtonClick<T>(
        this IContainer<T> updateContainer,
        TimeSpan timeOut,
        CallbackQueryRegex callbackQueryRegex,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default) where T : class
    {
        return await updateContainer.OpenChannel(
            new CallbackQueryChannel(
                timeOut,
                callbackQueryRegex),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
