using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Create an update container for any type of update.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultContainer<T> : AbstractUpdateContainer<T> where T : class
{
    internal DefaultContainer(
        Func<Update, T?> insiderResolver,
        IUpdater updater,
        ShiningInfo<long, Update> shiningInfo,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(insiderResolver, updater, shiningInfo, extraObjects)
    { }
}
