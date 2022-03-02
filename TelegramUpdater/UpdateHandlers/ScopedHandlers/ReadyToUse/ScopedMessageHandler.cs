namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    /// <summary>
    /// Abstract <see cref="IScopedUpdateHandler"/> for
    /// <see cref="Update.Message"/>.
    /// </summary>
    public abstract class ScopedMessageHandler : AnyScopedHandler<Message>
    {
        /// <summary>
        /// You can set handling priority in here.
        /// </summary>
        /// <param name="group">Handling priority.</param>
        protected ScopedMessageHandler(int group = 0)
            : base(x => x.Message, group)
        {
        }
    }
}
