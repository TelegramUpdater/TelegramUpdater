using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Base interface for scoped update handlers container.
/// </summary>
public interface IScopedUpdateHandlerContainer
{
    internal IReadOnlyDictionary<string, object>? ExtraData { get; }

    /// <summary>
    /// Type of <see cref="IScopedUpdateHandlerContainer"/>
    /// </summary>
    public Type ScopedHandlerType { get; }

    /// <summary>
    /// Your handler's update type.
    /// </summary>
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Checks if an update can be handled in a handler
    /// of type <see cref="ScopedHandlerType"/>.
    /// </summary>
    /// <returns></returns>
    public bool ShouldHandle(UpdaterFilterInputs<Update> inputs);

    /// <summary>
    /// Initialize an instance of <see cref="ScopedHandlerType"/>.
    /// </summary>
    /// <param name="scope">
    /// If there is any <see cref="IServiceProvider"/> and
    /// <see cref="IServiceScope"/>
    /// </param>
    /// <param name="logger"></param>
    /// <returns></returns>
    internal IScopedUpdateHandler? CreateInstance(
        IServiceScope? scope = default, ILogger? logger = default)
    {
        IScopedUpdateHandler? scopedHandler = null;

        try
        {
            if (scope != null)
            {
                scopedHandler = (IScopedUpdateHandler?)ActivatorUtilities.GetServiceOrCreateInstance(
                    scope.ServiceProvider, ScopedHandlerType);
            }
            else
            {
                scopedHandler = (IScopedUpdateHandler?)Activator
                    .CreateInstance(ScopedHandlerType);
            }
        }
        catch(Exception ex)
        {
            // Can't create an instance for any reason
            logger?.LogWarning(ex, "Can't create an instance of scoped handler: {handler}.", ScopedHandlerType.Name);
        }

        scopedHandler?.SetExtraData(ExtraData);
        return scopedHandler;
    }
}
