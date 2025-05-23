﻿using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.Poll"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="PollHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
/// <param name="endpoint">Determines if this is and endpoint handler.</param>
public class PollHandler(
    Func<IContainer<Poll>, Task> callback,
    IFilter<UpdaterFilterInputs<Poll>>? filter,
    bool endpoint = true)
    : DefaultHandler<Poll>(UpdateType.Poll, callback, filter, x => x.Poll, endpoint)
{
}
