using TelegramUpdater.RainbowUtilities;

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
    /// <param name="updater">
    /// Updater instance that handled this update.
    /// </param>
    /// <param name="shiningInfo">
    /// Information about an update that is processing.
    /// </param>
    /// <returns></returns>
    internal Task HandleAsync(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo);
}
