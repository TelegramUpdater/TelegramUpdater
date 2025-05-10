using Microsoft.Extensions.Primitives;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater;

/// <summary>
/// Raw inputs being passed to <see cref="IUpdateHandler.HandleAsync(HandlerInput)"/>.
/// </summary>
public class HandlerInput
{
    /// <summary>
    /// The updater.
    /// </summary>
    public IUpdater Updater { get; }

    /// <summary>
    /// Shining info contains information about the update and queuing.
    /// </summary>
    public ShiningInfo<long, Update> ShiningInfo { get; }

    /// <summary>
    /// The unique <see cref="Guid"/> of the scope of handling.
    /// </summary>
    /// <remarks>
    /// This scope id is same for handlers being triggered by the same update in handling chain.
    /// </remarks>
    public Guid ScopeId { get; }

    /// <summary>
    /// The <see cref="HandlingOptions.LayerInfo"/> of current handler.
    /// </summary>
    public LayerInfo LayerInfo { get; }

    /// <summary>
    /// The <see cref="HandlingOptions.Group"/> of this handler.
    /// </summary>
    public int Group { get; }

    /// <summary>
    /// Index of the handler in it's layer.
    /// </summary>
    public int Index { get; }

    internal IChangeToken? ScopeChangeToken { get; }

    internal IChangeToken? LayerChangeToken { get; }

    /// <summary>
    /// Create a new instance of <see cref="HandlerInput"/>.
    /// </summary>
    /// <param name="updater"></param>
    /// <param name="shiningInfo"></param>
    /// <param name="scopeId"></param>
    /// <param name="layerInfo"></param>
    /// <param name="group"></param>
    /// <param name="index"></param>
    public HandlerInput(
        IUpdater updater,
        ShiningInfo<long, Update> shiningInfo,
        Guid scopeId,
        LayerInfo layerInfo,
        int group,
        int index)
    {
        Updater = updater;
        ShiningInfo = shiningInfo;
        ScopeId = scopeId;
        LayerInfo = layerInfo;
        Group = group;
        Index = index;
    }

    internal HandlerInput(
        IUpdater updater,
        ShiningInfo<long, Update> shiningInfo,
        Guid scopeId,
        LayerInfo layerInfo,
        int group, 
        int index,
        IChangeToken? scopeChangeToken = default,
        IChangeToken? layerChangeToken = default)
    {
        Updater = updater;
        ShiningInfo = shiningInfo;
        ScopeId = scopeId;
        LayerInfo = layerInfo;
        Group = group;
        Index = index;
        ScopeChangeToken = scopeChangeToken;
        LayerChangeToken = layerChangeToken;
    }
}
