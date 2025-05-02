namespace TelegramUpdater.UpdateHandlers.Scoped.Attributes;

/// <summary>
/// Extra info about an <see cref="IScopedUpdateHandler"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ScopedHandlerAttribute : Attribute, IGetHandlingOptions
{
    /// <inheritdoc cref="HandlingOptions.Group"/>
    public int Group { get; set; } = default;

    /// <inheritdoc cref="HandlingOptions.LayerId"/>
    public object LayerId { get; set; } = HandlingOptions.DefaultLayer;

    /// <inheritdoc/>
    public HandlingOptions GetHandlingOptions()
        => new(group: Group, layerId: LayerId);
}
