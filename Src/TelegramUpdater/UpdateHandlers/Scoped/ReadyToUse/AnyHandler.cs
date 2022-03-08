﻿using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Creates an <see cref="IScopedUpdateHandler"/> for any type of update.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
public abstract class AnyHandler<T> : AbstractScopedUpdateHandler<T>
    where T : class
{
    internal AnyHandler(Func<Update, T?> getT, int group)
        : base(getT, group)
    {
    }

    /// <inheritdoc/>
    internal protected sealed override IContainer<T> ContainerBuilder(
        IUpdater updater, ShiningInfo<long, Update> shiningInfo)
    {
        return new AnyContainer<T>(GetT, updater, shiningInfo, ExtraData);
    }


    #region Extension Methods
    /// <summary>
    /// All pending handlers for this update will be ignored after throwing this.
    /// </summary>
    protected void StopPropagation() => Container.StopPropagation();

    /// <summary>
    /// Continue to the next pending handler for this update and ignore the rest of this handler.
    /// </summary>
    protected void ContinuePropagation() => Container.ContinuePropagation();
    #endregion
}


/*
 * All allowed update types since now.
 
    case UpdateType.Unknown:
    case UpdateType.MessageHandler:
    case UpdateType.InlineQueryHandler:
    case UpdateType.ChosenInlineResultHandler:
    case UpdateType.CallbackQueryHandler:
    case UpdateType.EditedMessageHandler:
    case UpdateType.ChannelPostHandler:
    case UpdateType.EditedChannelPostHandler:
    case UpdateType.ShippingQueryHandler:
    case UpdateType.PreCheckoutQueryHandler:
    case UpdateType.PollHandler:
    case UpdateType.PollAnswerHandler:
    case UpdateType.MyChatMemberHandler:
    case UpdateType.ChatMemberHandler:
    case UpdateType.ChatJoinRequestHandler:
 */