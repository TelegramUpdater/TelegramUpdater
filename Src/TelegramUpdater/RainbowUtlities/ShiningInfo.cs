namespace TelegramUpdater.RainbowUtlities
{
    /// <summary>
    /// Provides information about a <typeparamref name="TValue"/> while handling.
    /// </summary>
    /// <typeparam name="TId">The object's id.</typeparam>
    /// <typeparam name="TValue">Object's type</typeparam>
    public sealed class ShiningInfo<TId, TValue> where TId : struct
    {
        internal ShiningInfo(TValue value, Rainbow<TId, TValue> rainbow, ushort processId)
        {
            Value = value;
            Rainbow = rainbow;
            ProcessId = processId;
        }

        /// <summary>
        /// Orginal value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// <see cref="Rainbow{TId, TValue}"/>.
        /// </summary>
        public Rainbow<TId, TValue> Rainbow { get; }

        /// <summary>
        /// This is the id of the queue that contains this update.
        /// </summary>
        public ushort ProcessId { get; }
    }
}
