using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// This interface provides useful data for handlers.
/// </summary>
/// <typeparam name="TUpdate">
/// The type of inner actual update. One of <see cref="Update"/> properties.
/// </typeparam>
/// <typeparam name="TContainer"></typeparam>
public interface IHandlerProvider<TUpdate, TContainer>
    where TUpdate: class where TContainer : IContainer<TUpdate>
{
    /// <summary>
    /// The container.
    /// </summary>
    public TContainer Container { get; }
}
