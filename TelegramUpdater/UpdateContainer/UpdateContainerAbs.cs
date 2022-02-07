using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.UpdateContainer
{
    /// <summary>
    /// A container for incoming updates, which contains <typeparamref name="T"/> as your update.
    /// </summary>
    /// <typeparam name="T">Update type.</typeparam>
    public abstract class UpdateContainerAbs<T> : IUpdateContainer where T : class
    {
        private readonly Func<Update, T?> _insiderResovler;

        protected UpdateContainerAbs(
            Func<Update, T?> insiderResovler,
            Updater updater,
            Update insider,
            ITelegramBotClient botClient)
        {
            Updater = updater;
            Insider = insider;
            BotClient = botClient;
            _insiderResovler = insiderResovler;
        }

        public T Update
        {
            get
            {
                var inner = _insiderResovler(Insider);

                if (inner == null)
                    throw new InvalidOperationException(
                        $"Inner update should not be null! Excpected {typeof(T)} is {GetType()}");

                return inner;
            }
        }

        public Update Insider { get; }

        public ITelegramBotClient BotClient { get; }

        public Updater Updater { get; }


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
