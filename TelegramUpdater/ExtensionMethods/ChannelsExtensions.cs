using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateChannels.SealedChannels;

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
    /// <param name="abstractChannel">Your channel to choose the right update.</param>
    /// <param name="onUnrelatedUpdate">A callback function to be called if an unrelated update from comes.</param>
    /// <param name="cancellationToken">To cancell the job.</param>
    /// <returns></returns>
    public static async ValueTask<IContainer<T>?> OpenChannelAsync<T, K>(this IContainer<K> container,
                                                                         AbstractChannel<T>? abstractChannel = default,
                                                                         Func<IUpdater, ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
                                                                         CancellationToken cancellationToken = default)
        where T : class where K : class
    {
        if (container == null)
            throw new ArgumentNullException(nameof(container));

        if (abstractChannel == null)
        {
            throw new InvalidOperationException(
                "abstractChannel and updateResolver both can't be null");
        }

        while (true)
        {
            var update = await container.ShiningInfo.ReadNextAsync(abstractChannel.TimeOut, cancellationToken);

            if (update == null)
                return null;

            if (abstractChannel.UpdateType == update.Value.Type)
            {
                if (abstractChannel.ShouldChannel(update.Value))
                {
                    return abstractChannel.ContainerBuilder(container.Updater, update);
                }
            }

            if (onUnrelatedUpdate != null)
            {
                await onUnrelatedUpdate(container.Updater, update);
            }
        }
    }

    /// <summary>
    /// Opens a channel that dispatches a <see cref="Message"/> from updater.
    /// </summary>
    /// <param name="updateContainer">The update container</param>
    /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
    /// <param name="filter">Filter updates to get the right one.</param>
    /// <param name="onUnrelatedUpdate">A callback function to be called if an unrelated update from comes.</param>
    /// <param name="cancellationToken">To cancell the job.</param>
    public static async Task<IContainer<Message>?> ChannelMessage<K>(this IContainer<K> updateContainer,
                                                                     Filter<Message>? filter,
                                                                     TimeSpan? timeOut,
                                                                     Func<IUpdater, ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
                                                                     CancellationToken cancellationToken = default) where K : class
    {
        return await updateContainer.OpenChannelAsync(
            new MessageChannel(timeOut ?? TimeSpan.FromSeconds(30), filter),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken);
    }


    /// <summary>
    /// Opens a channel that dispatches a <see cref="Message"/> of this user from updater.
    /// </summary>
    /// <param name="updateContainer">The update container</param>
    /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
    /// <param name="filter">Filter updates to get the right one.</param>
    /// <param name="onUnrelatedUpdate">A callback function to be called if an unrelated update from comes.</param>
    /// <param name="cancellationToken">To cancell the job.</param>
    public static async Task<IContainer<Message>?> ChannelUserResponse(this IContainer<Message> updateContainer,
                                                                       TimeSpan timeOut,
                                                                       Func<IUpdater, ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
                                                                       Filter<Message>? filter = default,
                                                                       CancellationToken cancellationToken = default)
    {
        if (updateContainer.Update.From != null)
        {
            return await updateContainer.ChannelMessage(filter, timeOut, onUnrelatedUpdate, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("Sender can't be null!");
        }
    }

    public static async Task<IContainer<CallbackQuery>?> ChannelUserClick<T>(this IContainer<T> updateContainer,
                                                                             Func<T, long?> senderIdResolver,
                                                                             TimeSpan timeOut,
                                                                             string pattern,
                                                                             RegexOptions? regexOptions = default,
                                                                             Func<IUpdater, ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
                                                                             CancellationToken cancellationToken = default) where T : class
    {
        var senderId = senderIdResolver(updateContainer.Update);
        if (senderId != null)
        {
            return await updateContainer.OpenChannelAsync(
                new CallbackQueryChannel(
                    timeOut,
                    FilterCutify.CbqOfUsers(senderId.Value) &
                    FilterCutify.DataMatches(pattern, regexOptions)),
                onUnrelatedUpdate,
                cancellationToken: cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("Sender can't be null!");
        }
    }

    public static async Task<IContainer<CallbackQuery>?> ChannelUserClick(this IContainer<Message> updateContainer,
                                                                          TimeSpan timeOut,
                                                                          string pattern,
                                                                          RegexOptions? regexOptions = default,
                                                                          Func<IUpdater, ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
                                                                          CancellationToken cancellationToken = default)
    {
        return await updateContainer.ChannelUserClick(x => x.From?.Id ?? x.SenderChat?.Id ?? null,
                                                      timeOut,
                                                      pattern,
                                                      regexOptions,
                                                      onUnrelatedUpdate,
                                                      cancellationToken);
    }

    public static async Task<IContainer<CallbackQuery>?> ChannelUserClick(this IContainer<CallbackQuery> updateContainer,
                                                                          TimeSpan timeOut,
                                                                          string pattern,
                                                                          RegexOptions? regexOptions = default,
                                                                          Func<IUpdater, ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
                                                                          CancellationToken cancellationToken = default)
        => await updateContainer.ChannelUserClick(x => x.From.Id,
                timeOut, pattern, regexOptions, onUnrelatedUpdate, cancellationToken);
}
