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
        private readonly IReadOnlyDictionary<string, object> _extraObjects;

        internal UpdateContainerAbs(
            Func<Update, T?> insiderResovler,
            IUpdater updater,
            ShiningInfo<long, Update> shiningInsider,
            IReadOnlyDictionary<string, object>? extraObjects = default)
        {
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
            ShiningInfo = shiningInsider;
            Container = shiningInsider.Value;
            BotClient = updater.BotClient;
            _insiderResovler = insiderResovler ?? throw new ArgumentNullException(nameof(insiderResovler));
            _extraObjects = extraObjects?? new Dictionary<string, object>();
        }

        internal UpdateContainerAbs(
            Func<Update, T?> insiderResovler,
            IUpdater updater,
            Update insider,
            IReadOnlyDictionary<string, object>? extraObjects = default)
        {
            ShiningInfo = null!;
            Updater = updater;
            Container = insider;
            BotClient = updater.BotClient;
            _insiderResovler = insiderResovler;
            _extraObjects = extraObjects ?? new Dictionary<string, object>();
        }

        /// <inheritdoc/>
        public object this[string key] => _extraObjects[key];

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

        /// <inheritdoc/>
        public bool ContainsKey(string key) => _extraObjects.ContainsKey(key);
    }
}
