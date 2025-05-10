using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

namespace TelegramUpdater.UpdateHandlers.Singleton.Attributes;

/// <summary>
/// Place this attribute on any method to create an
/// <see cref="ISingletonUpdateHandler"/> using that method
/// as <see cref="IUpdateHandler.HandleAsync(HandlerInput)"/>.
/// <para>You can apply filter attributes on the method.</para>
/// </summary>
/// <remarks>
/// <b>NOTE:</b>
/// <list type="bullet">
/// <item>The method should return a <see cref="Task"/>.</item>
/// <item>The method should have only on parameter.</item>
/// <item>
/// Method's parameter should be <see cref="IContainer{T}"/>,
/// where T is the update type. Eg: <see cref="Message"/>
/// for a <see cref="MessageHandler"/>.
/// </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class SingletonHandlerCallbackAttribute : Attribute, IGetHandlingOptions
{
    /// <summary>
    /// Initialize a new instance of <see cref="SingletonHandlerCallbackAttribute"/>.
    /// </summary>
    /// <param name="updateType">Type of the update for the handler.</param>
    public SingletonHandlerCallbackAttribute(UpdateType updateType)
    {
        if (updateType == UpdateType.Unknown)
            throw new ArgumentException("Unknown? Really???", nameof(updateType));

        UpdateType = updateType;
    }

    /// <inheritdoc cref="HandlingOptions.Group"/>
    public int Group { get; set; } = default;

    /// <inheritdoc cref="LayerInfo.Key"/>
    public object? LayerKey { get; set; } = default;

    /// <inheritdoc cref="LayerInfo.Group"/>
    public int LayerGroup { get; set; } = default;

    /// <summary>
    /// Type of update.
    /// </summary>
    public UpdateType UpdateType { get; }

    /// <inheritdoc/>
    public HandlingOptions GetHandlingOptions()
        => new(
            group: Group,
            layerInfo: new LayerInfo(LayerKey?? HandlingOptions.DefaultLayerKey, LayerGroup));
}
