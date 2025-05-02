using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater;

/// <summary>
/// Raw inputs being passed to <see cref="IUpdateHandler.HandleAsync(IUpdater, HandlerInput)"/>.
/// </summary>
public class HandlerInput(
    IUpdater updater,
    ShiningInfo<long, Update> shiningInfo,
    Guid scopeId,
    int layerId,
    int group, 
    int index)
{
    /// <summary>
    /// The updater.
    /// </summary>
    public IUpdater Updater { get; } = updater;

    /// <summary>
    /// Shining info contains information about the update and queuing.
    /// </summary>
    public ShiningInfo<long, Update> ShiningInfo { get; } = shiningInfo;

    /// <summary>
    /// The unique <see cref="Guid"/> of the scope of handling.
    /// </summary>
    /// <remarks>
    /// This scope id is same for handlers being triggered by the same update in handling chain.
    /// </remarks>
    public Guid ScopeId { get; } = scopeId;

    /// <summary>
    /// The <see cref="HandlingOptions.LayerId"/> of current handler.
    /// </summary>
    public int LayerId { get; } = layerId;

    /// <summary>
    /// The <see cref="HandlingOptions.Group"/> of this handler.
    /// </summary>
    public int Group { get; } = group;

    /// <summary>
    /// Index of the handler in it's layer.
    /// </summary>
    public int Index { get; } = index;
}
