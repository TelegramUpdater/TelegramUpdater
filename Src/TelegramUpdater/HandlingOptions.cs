namespace TelegramUpdater;

/// <summary>
/// Handling information for layers.
/// </summary>
public class LayerInfo(int group = default)
{
    /// <summary>
    /// Handing priority of the layer.
    /// </summary>
    public int Group { get; } = group;
}

/// <summary>
/// Some more info about how a handler should be handled.
/// </summary>
/// <param name="group">Handling priority.</param>
/// <param name="layerInfo">
/// Handling layer info.
/// </param>
public class HandlingOptions(
    int group = default,
    LayerInfo? layerInfo = default)
{
    /// <summary>
    /// The default layer.
    /// </summary>
    public static readonly object DefaultLayerKey = new();

    /// <summary>
    /// Handling priority.
    /// </summary>
    /// <remarks>
    /// Handlers with lower <see cref="Group"/> will be handled sooner.
    /// </remarks>
    public int Group { get; } = group;

    /// <summary>
    /// Handling layer.
    /// </summary>
    /// <remarks>
    /// Layers define how separate handlers can affect each other.
    /// <para>
    /// Propagation in handlers like <see cref="StopPropagationException"/>
    /// only effects handlers in the same layer.
    /// </para>
    /// </remarks>
    public LayerInfo LayerInfo { get; } = layerInfo ?? new LayerInfo();

    /// <summary>
    /// Get input handling options or default.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static HandlingOptions OrDefault(HandlingOptions? options)
        => options ?? new();
}

internal interface IGetHandlingOptions
{
    public HandlingOptions GetHandlingOptions();
}