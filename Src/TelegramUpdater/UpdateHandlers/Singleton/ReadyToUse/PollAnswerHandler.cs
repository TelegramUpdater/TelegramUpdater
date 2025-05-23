﻿using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.PollAnswer"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="PollAnswerHandler"/>.
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
public class PollAnswerHandler(
    Func<IContainer<PollAnswer>, Task> callback,
    IFilter<UpdaterFilterInputs<PollAnswer>>? filter,
    bool endpoint = true)
    : DefaultHandler<PollAnswer>(UpdateType.PollAnswer, callback, filter, x => x.PollAnswer, endpoint)
{
}
