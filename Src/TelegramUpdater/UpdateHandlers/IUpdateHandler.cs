using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// Base interface for all update handlers.
/// </summary>
public interface IUpdateHandler
{
    /// <summary>
    /// The container available inside the handler.
    /// </summary>
    IContainer RawContainer { get; }

    /// <summary>
    /// Determines if this handler is an end point for it's layer.
    /// </summary>
    public bool Endpoint { get; }

    /// <summary>
    /// Handle the update.
    /// </summary>
    public Task HandleAsync(
        HandlerInput input,
        IServiceScope? scope = default,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Base interface for all update handlers.
/// </summary>
public interface IUpdateHandler<T>: IUpdateHandler where T: class
{
    /// <summary>
    /// The container available inside the handler.
    /// </summary>
    IContainer<T> TypedRawContainer { get; }
}
