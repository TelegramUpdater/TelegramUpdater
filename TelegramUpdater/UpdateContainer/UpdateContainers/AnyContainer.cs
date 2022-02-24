using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    /// <summary>
    /// Create an update container for any type of update.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnyContainer<T> : UpdateContainerAbs<T> where T : class
    {
        internal AnyContainer(
            Func<Update, T?> insiderResovler,
            IUpdater updater,
            ShiningInfo<long, Update> shiningInfo)
            : base(insiderResovler, updater, shiningInfo)
        { }
    }
}
