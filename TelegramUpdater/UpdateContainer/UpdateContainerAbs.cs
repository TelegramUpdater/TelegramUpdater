using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateContainer
{
    /// <summary>
    /// A container for incoming updates, which contains <typeparamref name="T"/> as your update.
    /// </summary>
    /// <typeparam name="T">Update type.</typeparam>
    public abstract class UpdateContainerAbs<T> : IContainer<T> where T : class
    {
        private readonly Func<Update, T?> _insiderResovler;

        protected UpdateContainerAbs(
            Func<Update, T?> insiderResovler,
            IUpdater updater,
            Update insider)
        {
            Updater = updater;
            Container = insider;
            BotClient = updater.BotClient;
            _insiderResovler = insiderResovler;
        }

        public T Update
        {
            get
            {
                var inner = _insiderResovler(Container);

                if (inner == null)
                    throw new InvalidOperationException(
                        $"Inner update should not be null! Excpected {typeof(T)} is {GetType()}");

                return inner;
            }
        }
  
        /// <inheritdoc/>
        public Update Container { get; }

        /// <inheritdoc/>
        public ITelegramBotClient BotClient { get; }

        /// <inheritdoc/>
        public IUpdater Updater { get; }
    }
}
