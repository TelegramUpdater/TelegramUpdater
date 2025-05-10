using Microsoft.Extensions.DependencyInjection;

namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// Base interface for all update handlers.
/// </summary>
public interface IUpdateHandler
{
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
