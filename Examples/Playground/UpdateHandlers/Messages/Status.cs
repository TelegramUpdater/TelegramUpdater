using Microsoft.EntityFrameworkCore;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Filters;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.Attributes;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using static TelegramUpdater.UpdaterExtensions;

namespace Playground.UpdateHandlers.Messages;

[Command("status"), Private]
[ScopedHandler(Group = 0, LayerKey = 1)]
internal class StatusFilter(PlaygroundMemory memory) : MessageHandler
{
    // This is not the end
    public override bool Endpoint => false;

    protected override async Task HandleAsync(MessageContainer container)
    {
        if (From == null)
            StopPropagation();

        container.SetScopeItem("scopeObject", new object());
        container.SetLayerItem("layerObject", new object());

        if (!await memory.SeenUsers.AnyAsync(x=> x.TelegramId == From.Id))
        {
            StopPropagation();
        }

        // If there's no user or, the user has not been seen stop handling
        // reaming handlers in this layer
    }
}

[Command("status"), Private]
[ScopedHandler(Group = 1, LayerKey = 1)]
internal class StatusSeen(ILogger<StatusSeen> logger) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        if (From is null) return;

        if (!container.TryGetScopeItem("scopeObject", out object? _))
            logger.LogCritical("I MUST have access to 'scopeObject' since we are on a same scope.");

        if (!container.TryGetLayerItem("layerObject", out object? _))
            logger.LogCritical("I MUST have access to 'layerObject' since we are on a same layer.");

        container.SetUserScopeItem("seen", true);
        await Response("Hooray! you have been seen");
    }
}

[Command("status"), Private]
[UserUpdaterDataExists("seen", Region = DataRegion.Scope, ThenRemove = true, Reverse = true)]
[ScopedHandler(LayerGroup = 2)] // layer 2 ensures this runs after StatusSeen which is on layer 1
internal class StatusNotSeen(ILogger<StatusNotSeen> logger) : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        if (From is null) return;

        if (!container.TryGetScopeItem("scopeObject", out object? _))
            logger.LogCritical("I MUST have access to 'scopeObject' since we are on a same scope.");

        if (container.TryGetLayerItem("layerObject", out object? _))
            logger.LogCritical("I MUST NOT have access to 'layerObject' since we are not on a same layer.");

        if (container.TryGetUserScopeItem("seen", out bool? _))
            logger.LogCritical("I MUST NOT have access to 'seen' because of ThenRemove = true.");

        await Response("Oh! i have not seen you yet");
    }
}
