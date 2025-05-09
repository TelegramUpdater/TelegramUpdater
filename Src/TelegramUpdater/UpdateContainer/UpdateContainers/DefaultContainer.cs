namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Create an update container for any type of update.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultContainer<T> : AbstractUpdateContainer<T> where T : class
{
    internal DefaultContainer(
        Func<Update, T?> insiderResolver,
        HandlerInput input,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(insiderResolver, input, extraObjects)
    { }
}
