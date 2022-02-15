using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateChannels.SealedChannels;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater
{
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
        /// <param name="updateResolver">Fill this if you left <paramref name="abstractChannel"/> empty.</param>
        /// <param name="timeOut">Maximum timeOut to wait for that update</param>
        /// <param name="cancellationToken">To cancell the job.</param>
        /// <returns></returns>
        public static async ValueTask<IContainer<T>?> OpenChannelAsync<T, K>(
            this IContainer<K> container,
            TimeSpan timeOut,
            AbstractChannel<T>? abstractChannel = default,
            Func<Update, T>? updateResolver = default,
            CancellationToken cancellationToken = default)
            where T: class where K: class
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (timeOut == default)
                throw new ArgumentException("Use a valid time out.");

            if (abstractChannel == null && updateResolver == null)
            {
                throw new InvalidOperationException(
                    "abstractChannel and updateResolver both can't be null");
            }

            while (true)
            {
                var update = await container.ShiningInfo.ReadNextAsync(timeOut, cancellationToken);

                if (update == null)
                    return null;

                if (abstractChannel == null)
                {
                    return new AnyContainer<T>(updateResolver!, container.Updater, update);
                }

                if (abstractChannel.UpdateType == update.Value.Type)
                {
                    if (abstractChannel.ShouldChannel(update.Value))
                    {
                        return abstractChannel.ContainerBuilder(container.Updater, update);
                    }
                }
            }
        }

        /// <summary>
        /// Opens a channel that dispatches a <see cref="Message"/> from updater.
        /// </summary>
        /// <param name="updateContainer">The update container</param>
        /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
        /// <param name="filter">Filter updates to get the right one.</param>
        /// <param name="cancellationToken">To cancell the job.</param>
        public static async Task<IContainer<Message>?> ChannelMessage<K>(
            this IContainer<K> updateContainer,
            Filter<Message>? filter,
            TimeSpan? timeOut,
            CancellationToken cancellationToken = default) where K: class
        {
            return await updateContainer.OpenChannelAsync(
                timeOut ?? TimeSpan.FromSeconds(30),
                new MessageChannel(filter),
                cancellationToken: cancellationToken);
        }


        /// <summary>
        /// Opens a channel that dispatches a <see cref="Message"/> of this user from updater.
        /// </summary>
        /// <param name="updateContainer">The update container</param>
        /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
        /// <param name="filter">Filter updates to get the right one.</param>
        public static async Task<IContainer<Message>?> ChannelUserResponse(
            this IContainer<Message> updateContainer,
            TimeSpan timeOut,
            Filter<Message>? filter = default)
        {
            if (updateContainer.Update.From != null)
            {
                var realFilter = FilterCutify.MsgOfUsers(updateContainer.Update.From.Id);
                if (filter != null)
                {
                    realFilter &= filter;
                }

                return await updateContainer.ChannelMessage(realFilter, timeOut);
            }
            else
            {
                throw new InvalidOperationException("Sender can't be null!");
            }
        }

        public static async Task<IContainer<CallbackQuery>?> ChannelUserClick<T>(
            this IContainer<T> updateContainer,
            Func<T, long?> senderIdResolver,
            TimeSpan timeOut,
            string pattern,
            RegexOptions? regexOptions = default,
            CancellationToken cancellationToken = default) where T : class
        {
            var senderId = senderIdResolver(updateContainer.Update);
            if (senderId != null)
            {
                return await updateContainer.OpenChannelAsync(
                    timeOut,
                    new CallbackQueryChannel(
                        FilterCutify.CbqOfUsers(senderId.Value) &
                        FilterCutify.DataMatches(pattern, regexOptions)),
                    cancellationToken: cancellationToken);
            }
            else
            {
                throw new InvalidOperationException("Sender can't be null!");
            }
        }

        public static async Task<IContainer<CallbackQuery>?> ChannelUserClick(
            this IContainer<Message> updateContainer,
            TimeSpan timeOut,
            string pattern,
            RegexOptions? regexOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await updateContainer.ChannelUserClick(x => x.From?.Id ?? x.SenderChat?.Id ?? null,
                                                          timeOut,
                                                          pattern,
                                                          regexOptions,
                                                          cancellationToken);
        }

        public static async Task<IContainer<CallbackQuery>?> ChannelUserClick(
            this IContainer<CallbackQuery> updateContainer,
            TimeSpan timeOut,
            string pattern,
            RegexOptions? regexOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await updateContainer.ChannelUserClick(x => x.From.Id,
                                                          timeOut,
                                                          pattern,
                                                          regexOptions,
                                                          cancellationToken);
        }
    }
}
