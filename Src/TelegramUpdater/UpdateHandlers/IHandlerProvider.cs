using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// This interface provides useful data for handlers.
/// </summary>
/// <typeparam name="TUpdate">
/// The type of inner actual update. One of <see cref="Update"/> properties.
/// </typeparam>
public interface IHandlerProvider<TUpdate> where TUpdate: class
{
    /// <summary>
    /// The container.
    /// </summary>
    public IContainer<TUpdate> Container { get; }
}
