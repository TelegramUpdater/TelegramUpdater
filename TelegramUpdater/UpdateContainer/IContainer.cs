namespace TelegramUpdater.UpdateContainer
{
    /// <summary>
    /// A sub interface of <see cref="IUpdateContainer"/>, made for simplicity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T> : IUpdateContainer where T : class
    {
        /// <summary>
        /// The actual update.
        /// </summary>
        public T Update { get; }
    }
}
