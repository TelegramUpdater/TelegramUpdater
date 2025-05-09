using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Creates an <see cref="IScopedUpdateHandler"/> for any type of update.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
/// <remarks>
/// Create a new instance of <see cref="DefaultHandler{T}"/>.
/// </remarks>
/// <param name="getT">To extract <typeparamref name="T"/> from <see cref="Update"/>.</param>
public abstract class DefaultHandler<T>(Func<Update, T?> getT)
    : AbstractScopedUpdateHandler<T, DefaultContainer<T>>(getT)
    where T : class
{
    /// <inheritdoc/>
    protected internal sealed override DefaultContainer<T> ContainerBuilder(HandlerInput input)
        => new(GetT, input, ExtraData);
}
