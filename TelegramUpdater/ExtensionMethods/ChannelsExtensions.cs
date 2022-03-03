using TelegramUpdater.Filters;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateChannels.SealedChannels;
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
    /// <typeparam name="T">Update type you're excpecting.</typeparam>
    /// <typeparam name="K">Current container update type.</typeparam>
    /// <param name="container">The container itself.</param>
    /// <param name="updateChannel">
    /// Your channel to choose the right update.
    /// </param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancell the job.</param>
    /// <returns></returns>
    public static async ValueTask<IContainer<T>?> OpenChannelAsync<T, K>(
        this IContainer<K> container,
        IGenericUpdateChannel<T> updateChannel,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
        where T : class where K : class
    {
        if (container == null)
            throw new ArgumentNullException(nameof(container));

        if (updateChannel == null)
        {
            throw new InvalidOperationException(
                "abstractChannel and updateResolver both can't be null");
        }

        // A secondary timeOut, cuz ReadNextAsync'timeout will reset on unrelated update.
        var timeOutCts = new CancellationTokenSource();
        timeOutCts.CancelAfter(updateChannel.TimeOut);

        using var linkedCts = CancellationTokenSource
            .CreateLinkedTokenSource(timeOutCts.Token, cancellationToken);

        while (true)
        {
            try
            {
                var update = await container.ShiningInfo.ReadNextAsync(
                    updateChannel.TimeOut, linkedCts.Token);

                if (update == null)
                    return null;

                if (updateChannel.ShouldChannel(update.Value))
                {
                    return new AnyContainer<T>(
                        updateChannel.GetActualUpdate,
                        container.Updater,
                        update,
                        updateChannel.ExtraData
                    );
                }

                if (onUnrelatedUpdate != null)
                {
                    await onUnrelatedUpdate(container.Updater, update);
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
    /// <param name="cancellationToken">To cancell the job.</param>
    public static async Task<IContainer<Message>?> ChannelMessage<K>(
        this IContainer<K> updateContainer,
        Filter<Message>? filter,
        TimeSpan? timeOut,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default) where K : class
    {
        return await updateContainer.OpenChannelAsync(
            new MessageChannel(timeOut ?? TimeSpan.FromSeconds(30), filter),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken);
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
    /// <param name="cancellationToken">To cancell the job.</param>
    public static async Task<IContainer<Message>?> ChannelUserResponse(
        this IContainer<Message> updateContainer,
        TimeSpan timeOut,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        Filter<Message>? filter = default,
        CancellationToken cancellationToken = default)
    {
        return await updateContainer.ChannelMessage(
            filter, timeOut, onUnrelatedUpdate, cancellationToken);
    }

    /// <summary>
    /// Waits for the user to click on an inline button.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static async Task<IContainer<CallbackQuery>?> ChannelUserClick<T>(
        this IContainer<T> updateContainer,
        TimeSpan timeOut,
        CallbackQueryRegex callbackQueryRegex,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default) where T : class
    {
        return await updateContainer.OpenChannelAsync(
            new CallbackQueryChannel(
                timeOut,
                callbackQueryRegex),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken);
    }
}
