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
            Updater updater,
            Update insider,
            ITelegramBotClient botClient)
        {
            Updater = updater;
            Container = insider;
            BotClient = botClient;
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

        public Update Container { get; }

        public ITelegramBotClient BotClient { get; }

        public Updater Updater { get; }
    }
}
