namespace TelegramUpdater.UpdateHandlers.Scoped.Attributes;

/// <summary>
/// Extra info about an <see cref="IScopedUpdateHandler"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ScopedHandlerAttribute : Attribute, IGetHandlingOptions
{
    /// <inheritdoc cref="HandlingOptions.Group"/>
    public int Group { get; set; } = default;

    /// <inheritdoc cref="LayerInfo.Key"/>
    public object? LayerKey { get; set; } = default;

    /// <inheritdoc cref="LayerInfo.Group"/>
    public int LayerGroup { get; set; } = default;

    /// <inheritdoc/>
    public HandlingOptions GetHandlingOptions()
        => new(
            group: Group,
            layerInfo: new LayerInfo(LayerKey ?? HandlingOptions.DefaultLayerKey, LayerGroup));
}
