namespace TelegramUpdater.UpdateHandlers.Singleton;

/// <summary>
/// A generic interface over <see cref="ISingletonUpdateHandler"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGenericSingletonUpdateHandler<T>
    : ISingletonUpdateHandler, IUpdateHandler<T> where T : class
{
    /// <summary>
    /// Filter for this handler.
    /// </summary>
    public IFilter<UpdaterFilterInputs<T>>? Filter { get; }

    /// <summary>
    /// A function to extract actual update from <see cref="Update"/>.
    /// </summary>
    /// <remarks>
    /// The inner update will be resolved from <see cref="UpdateType"/> if this is null.
    /// </remarks>
    public Func<Update, T?>? GetActualUpdate { get; }
}
