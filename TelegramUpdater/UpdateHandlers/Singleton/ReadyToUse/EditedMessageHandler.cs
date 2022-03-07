﻿using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.EditedMessage"/>.
/// </summary>
public sealed class EditedMessageHandler : AnyHandler<Message>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="EditedMessageHandler"/>.
    /// </summary>
    /// <param name="callback">
    /// A callback function that will be called when an <see cref="Update"/>
    /// passes the <paramref name="filter"/>.
    /// </param>
    /// <param name="filter">
    /// A filter to choose the right update to be handled inside
    /// <paramref name="callback"/>.
    /// </param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public EditedMessageHandler(Func<IContainer<Message>, Task> callback,
                                IFilter<Message>? filter,
                                int group)
        : base(UpdateType.EditedMessage, x=> x.EditedMessage, callback, filter, group)
    {
    }
}
