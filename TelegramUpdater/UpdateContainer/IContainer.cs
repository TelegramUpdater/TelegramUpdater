using System;
using System.Threading.Tasks;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.UpdateContainer
{
    public interface IContainer<T> : IUpdateContainer where T : class
    {
        /// <summary>
        /// The real update.
        /// </summary>
        public T Update { get; }

        /// <summary>
        /// Opens a channel through the update handler and reads specified update.
        /// </summary>
        /// <typeparam name="K">Type of update you're excepting.</typeparam>
        /// <param name="updateChannel">An <see cref="IUpdateChannel"/></param>
        /// <param name="timeOut">Maximum allowed time to wait for that update.</param>
        public async Task<K?> OpenChannel<K>(AbstractChannel<K> updateChannel, TimeSpan timeOut)
            where K : class
        {
            return await Updater.OpenChannel(updateChannel, timeOut);
        }
    }
}
