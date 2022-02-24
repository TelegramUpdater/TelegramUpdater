using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateHandlers
{
    /// <summary>
    /// Baes interface for all update handlers.
    /// </summary>
    public interface IUpdateHandler
    {
        /// <summary>
        /// Handling priority of this.
        /// </summary>
        public int Group { get; }

        /// <summary>
        /// Handle the update.
        /// </summary>
        /// <param name="updater">Updater instance that handled this update.</param>
        /// <param name="shiningInfo">Information about an update that is processing.</param>
        /// <returns></returns>
        public Task HandleAsync(IUpdater updater, ShiningInfo<long, Update> shiningInfo);
    }
}
