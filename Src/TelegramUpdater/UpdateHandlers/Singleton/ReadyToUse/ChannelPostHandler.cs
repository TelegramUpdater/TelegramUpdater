﻿using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChannelPost"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="ChannelPostHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
/// <param name="endpoint"></param>
public class ChannelPostHandler(
    Func<MessageContainer, Task> callback,
    IFilter<UpdaterFilterInputs<Message>>? filter,
    bool endpoint = true)
    : AbstractMessageHandler(
        UpdateType.ChannelPost,
        callback,
        filter,
        x => x.ChannelPost,
        endpoint)
{
}
