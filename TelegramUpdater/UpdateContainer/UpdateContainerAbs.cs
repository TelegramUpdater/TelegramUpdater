using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer
{
    /// <summary>
    /// A container for incoming updates, which contains <typeparamref name="T"/> as your update.
    /// </summary>
    /// <typeparam name="T">Update type.</typeparam>
    public abstract class UpdateContainerAbs<T> : IContainer<T> where T : class
    {
        private readonly Func<Update, T?> _insiderResovler;

        internal UpdateContainerAbs(
            Func<Update, T?> insiderResovler,
            IUpdater updater,
            ShiningInfo<long, Update> insider)
        {
            Updater = updater;
            ShiningInfo = insider;
            Container = insider.Value;
            BotClient = updater.BotClient;
            _insiderResovler = insiderResovler;
        }

        internal UpdateContainerAbs(
            Func<Update, T?> insiderResovler,
            IUpdater updater,
            Update insider)
        {
            ShiningInfo = null!;
            Updater = updater;
            Container = insider;
            BotClient = updater.BotClient;
            _insiderResovler = insiderResovler;
        }

        /// <summary>
        /// Orginal update. ( inner update, like <see cref="Update.Message"/> ) 
        /// </summary>
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
        public virtual Update Container { get; }

        /// <inheritdoc/>
        public virtual ITelegramBotClient BotClient { get; }

        /// <inheritdoc/>
        public virtual ShiningInfo<long, Update> ShiningInfo { get; }

        /// <inheritdoc/>
        public virtual IUpdater Updater { get; }
    }
}
