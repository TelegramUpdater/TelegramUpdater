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
            Func<Update, T?> insiderResolver,
            IUpdater updater,
            ShiningInfo<long, Update> shiningInfo,
            IReadOnlyDictionary<string, object>? extraObjects = default)
            : base(insiderResolver, updater, shiningInfo, extraObjects)
        { }
    }
}
