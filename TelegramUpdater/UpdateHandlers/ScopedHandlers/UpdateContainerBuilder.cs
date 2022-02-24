namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Builds containers for <see cref="IScopedUpdateHandler"/>s.
    /// </summary>
    /// <typeparam name="THandler">The handler, which is <see cref="IScopedUpdateHandler"/></typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    public sealed class UpdateContainerBuilder<THandler, TUpdate>
        : AbstractUpdateContainer<THandler, TUpdate>
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        private readonly Func<Update, TUpdate?>? _getT;

        /// <summary>
        /// Create an instance of scoped update handler container builder.
        /// </summary>
        /// <remarks>
        /// In orther to use <see cref="IScopedUpdateHandler"/>s in <see cref="IUpdater"/>, you need
        /// to place them inside an <see cref="IScopedHandlerContainer"/>. this class dose that.
        /// </remarks>
        /// <param name="updateType">The type of update,</param>
        /// <param name="filter">Your filter.</param>
        /// <param name="getT">A function to resolve inner update from <see cref="Update"/>.</param>
        public UpdateContainerBuilder(
            UpdateType updateType,
            Filter<TUpdate>? filter = default,
            Func<Update, TUpdate?>? getT = default)
            : base(updateType, filter)
        {
            _getT = getT;
        }

        /// <summary>
        /// A function to resolve inner update from <see cref="Update"/>.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        protected override TUpdate? GetT(Update update)
        {
            if (_getT == null)
            {
                return update.GetInnerUpdate<TUpdate>();
            }
            else
            {
                return _getT(update);
            }
        }
    }
}
