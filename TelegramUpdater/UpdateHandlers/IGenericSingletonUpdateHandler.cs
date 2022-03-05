namespace TelegramUpdater.UpdateHandlers
{
    /// <summary>
    /// A generic interface over <see cref="ISingletonUpdateHandler"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericSingletonUpdateHandler<T> : ISingletonUpdateHandler
        where T : class
    {
        /// <summary>
        /// Filter for this handler.
        /// </summary>
        public IFilter<T>? Filter { get; }

        /// <summary>
        /// A function to extract actual update from <see cref="Update"/>.
        /// </summary>
        public Func<Update, T?> GetActualUpdate { get; }
    }
}
