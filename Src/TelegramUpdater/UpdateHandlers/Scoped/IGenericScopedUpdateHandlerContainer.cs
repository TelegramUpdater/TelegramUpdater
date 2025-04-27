namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Generic interface over <see cref="IScopedUpdateHandlerContainer"/>.
/// </summary>
public interface IGenericScopedUpdateHandlerContainer<T> : IScopedUpdateHandlerContainer
    where T : class
{
    /// <summary>
    /// Filter for this handler.
    /// </summary>
    public IFilter<UpdaterFilterInputs<T>>? Filter { get; }
}
