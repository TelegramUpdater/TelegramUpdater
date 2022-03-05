namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Generic interface over <see cref="IScopedUpdateHandlerContainer"/>.
    /// </summary>
    public interface IGenericScopedUpdateHandlerContainer<T> : IScopedUpdateHandlerContainer
        where T : class
    {
        /// <summary>
        /// Filter for this handler.
        /// </summary>
        public IFilter<T>? Filter { get; }
    }
}
