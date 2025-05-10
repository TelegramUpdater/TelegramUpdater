namespace TelegramUpdater.UpdateHandlers.Singleton;

/// <summary>
/// Interface for normal update handler (known as singleton handlers)
/// </summary>
public interface ISingletonUpdateHandler : IUpdateHandler, IHandlerFiltering
{

}
