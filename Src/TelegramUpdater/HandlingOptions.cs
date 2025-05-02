namespace TelegramUpdater;

/// <summary>
/// Some more info about how a handler should be handled.
/// </summary>
/// <param name="group">Handling priority.</param>
/// <param name="layerId">
/// Handling layer.
/// </param>
public class HandlingOptions(
    int group = 0,
    object? layerId = default)
{
    /// <summary>
    /// The default layer.
    /// </summary>
    public static readonly object DefaultLayer = new();

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
    public object LayerId { get; } = layerId?? DefaultLayer;

    /// <summary>
    /// Get input handling options or default.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static HandlingOptions OrDefault(HandlingOptions? options)
        => options ?? new(layerId: DefaultLayer);
}

internal interface IGetHandlingOptions
{
    public HandlingOptions GetHandlingOptions();
}