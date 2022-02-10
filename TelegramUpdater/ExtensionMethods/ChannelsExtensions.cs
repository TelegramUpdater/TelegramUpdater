using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateChannels.SealedChannels;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater
{
    public static class ChannelsExtensions
    {
        /// <summary>
        /// Opens a channel that dispatches a <see cref="Message"/> from updater.
        /// </summary>
        /// <param name="updateContainer">The update container</param>
        /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
        /// <param name="filter">Filter updates to get the right one.</param>
        public static async Task<IContainer<Message>?> ChannelMessage(this IContainer<Message> updateContainer,
                                                          Filter<Message>? filter,
                                                          TimeSpan? timeOut = default)
        {
            var message = await updateContainer.OpenChannel(
                  new MessageChannel(filter), timeOut ?? TimeSpan.FromSeconds(30));
            if (message != null)
                return message.Wrap(updateContainer);
            return null;
        }


        /// <summary>
        /// Opens a channel that dispatches a <see cref="Message"/> of this user from updater.
        /// </summary>
        /// <param name="updateContainer">The update container</param>
        /// <param name="timeOut">Maximum allowed time to wait for the update.</param>
        /// <param name="filter">Filter updates to get the right one.</param>
        public static async Task<IContainer<Message>?> ChannelUserResponse(this IContainer<Message> updateContainer,
                                                          Filter<Message>? filter = default,
                                                          TimeSpan? timeOut = default)
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

        public static async Task<IContainer<CallbackQuery>?> ChannelUserClick<T>(this IContainer<T> updateContainer,
                                                                                 Func<T, long?> senderIdResolver,
                                                                                 TimeSpan timeOut,
                                                                                 string pattern,
                                                                                 RegexOptions? regexOptions = default) where T : class
        {
            var senderId = senderIdResolver(updateContainer.Update);
            if (senderId != null)
            {
                var result = await updateContainer.Updater.OpenChannel(
                    new CallbackQueryChannel(
                        FilterCutify.CbqOfUsers(senderId.Value) &
                        FilterCutify.DataMatches(pattern, regexOptions)),
                    timeOut);

                if (result != null)
                    return result.Wrap(updateContainer);
                return null;
            }
            else
            {
                throw new InvalidOperationException("Sender can't be null!");
            }
        }

        public static async Task<IContainer<CallbackQuery>?> ChannelUserClick(this IContainer<Message> updateContainer,
                                                                              TimeSpan timeOut,
                                                                              string pattern,
                                                                              RegexOptions? regexOptions = default)
        {
            return await updateContainer.ChannelUserClick(x => x.From?.Id ?? x.SenderChat?.Id ?? null,
                                                          timeOut,
                                                          pattern,
                                                          regexOptions);
        }

        public static async Task<IContainer<CallbackQuery>?> ChannelUserClick(this IContainer<CallbackQuery> updateContainer,
                                                                              TimeSpan timeOut,
                                                                              string pattern,
                                                                              RegexOptions? regexOptions = default)
        {
            return await updateContainer.ChannelUserClick(x => x.From.Id,
                                                          timeOut,
                                                          pattern,
                                                          regexOptions);
        }
    }
}
