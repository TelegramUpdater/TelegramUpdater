using System.ComponentModel;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Controller.Attributes;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// This is a default controller update handler.
/// </summary>
/// <remarks>
/// After inheriting from this class, you can add methods with <see cref="HandlerActionAttribute"/> to handle them.
/// You can also add filters to the methods using attributes.
/// <para>
/// Method parameters can be resolved using <see cref="ResolveServiceAttribute"/>. or can be <see cref="IContainer"/>
/// </para>
/// </remarks>
/// <typeparam name="T"></typeparam>
/// <param name="getT"></param>
public abstract class DefaultControllerHandler<T>(Func<Update, T?>? getT = default)
    : AbstractControllerUpdateHandler<T, DefaultContainer<T>>(getT) where T : class
{
    /// <inheritdoc/>
    protected internal override DefaultContainer<T> ContainerBuilder(HandlerInput input)
    {
        return new(GetT, input, ExtraData);
    }
}
