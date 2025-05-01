using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater;

/// <summary>
/// A set of useful extension methods for <see cref="IScopedUpdateHandler"/>s.
/// </summary>
public static class ScopedUpdateHandlersExtensions
{
    /// <summary>
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="typeOfScopedHandler">Type of your handler.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <param name="group"></param>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<TUpdate>(
        this IUpdater updater,
        Type typeOfScopedHandler,
        UpdateType updateType,
        UpdaterFilter<TUpdate>? filter = default,
        Func<Update, TUpdate>? getT = default,
        int group = default) where TUpdate : class
    {
        if (!typeof(IScopedUpdateHandler).IsAssignableFrom(typeOfScopedHandler))
        {
            throw new InvalidCastException(
                $"{typeOfScopedHandler} Should be an IScopedUpdateHandler");
        }

        var _t = typeof(TUpdate);

        var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>).MakeGenericType(
            typeOfScopedHandler, _t);

        var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
            containerGeneric, [updateType, filter, getT]);

        if (container != null)
        {
            return updater.AddScopedUpdateHandler(container, group);
        }

        updater.Logger.LogWarning(
            "{type} not added to the Scoped Handlers! The instance of it is null.",
            typeOfScopedHandler);
        throw new InvalidOperationException(
            "Handler not added to the Scoped Handlers! The instance of it is null.");
    }

    /// <summary>
    /// Adds an scoped handler to the updater. ( Use this if you're not sure. )
    /// </summary>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <param name="group">Handling priority.</param>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<THandler, TUpdate>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<TUpdate>>? filter = default,
        Func<Update, TUpdate?>? getT = default,
        int group = default)
        where THandler : AbstractScopedUpdateHandler<TUpdate> where TUpdate : class
    {
        var _h = typeof(THandler);

        return updater.AddScopedUpdateHandler(
            new ScopedUpdateHandlerContainerBuilder<THandler, TUpdate>(
                updateType, filter, getT), group);
    }

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for any update type that is <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updateType">
    /// Type of update. should be one of <see cref="UpdateType.Message"/>,
    /// <see cref="UpdateType.EditedMessage"/>, <see cref="UpdateType.ChannelPost"/>,
    /// <see cref="UpdateType.EditedChannelPost"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<Message>>? filter = default)
        where THandler : AbstractScopedUpdateHandler<Message>
        => updater.AddScopedUpdateHandler<THandler, Message>(
            updateType, filter, updateType switch
            {
                UpdateType.Message => x => x.Message,
                UpdateType.EditedMessage => x => x.EditedMessage,
                UpdateType.ChannelPost => x => x.ChannelPost,
                UpdateType.EditedChannelPost => x => x.EditedChannelPost,

                // New updates
                UpdateType.BusinessMessage => x => x.BusinessMessage,
                UpdateType.EditedBusinessMessage => x => x.EditedBusinessMessage,

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a Message.",
                    nameof(updateType)
                ),
            });

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for any update type that is <see cref="ChatMemberUpdated"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updateType">
    /// Type of update. should be one of
    /// <see cref="UpdateType.ChatMember"/>, <see cref="UpdateType.MyChatMember"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<ChatMemberUpdated>>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatMemberUpdated>
        => updater.AddScopedUpdateHandler<THandler, ChatMemberUpdated>(
            updateType, filter, updateType switch
            {
                UpdateType.ChatMember => x => x.ChatMember,
                UpdateType.MyChatMember => x => x.MyChatMember,

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated.",
                    nameof(updateType)
                ),
            });

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<CallbackQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<CallbackQuery>
        => updater.AddScopedUpdateHandler<THandler, CallbackQuery>(
            UpdateType.CallbackQuery, filter, x => x.CallbackQuery);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.InlineQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<InlineQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<InlineQuery>
        => updater.AddScopedUpdateHandler<THandler, InlineQuery>(
            UpdateType.InlineQuery, filter, x => x.InlineQuery);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ChatJoinRequest"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatJoinRequest>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatJoinRequest>
        => updater.AddScopedUpdateHandler<THandler, ChatJoinRequest>(
            UpdateType.ChatJoinRequest, filter, x => x.ChatJoinRequest);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ChosenInlineResult"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChosenInlineResult>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChosenInlineResult>
        => updater.AddScopedUpdateHandler<THandler, ChosenInlineResult>(
            UpdateType.ChosenInlineResult, filter, x => x.ChosenInlineResult);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.Poll"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<Poll>? filter = default)
        where THandler : AbstractScopedUpdateHandler<Poll>
        => updater.AddScopedUpdateHandler<THandler, Poll>(
            UpdateType.Poll, filter, x => x.Poll);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.PollAnswer"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PollAnswer>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PollAnswer>
        => updater.AddScopedUpdateHandler<THandler, PollAnswer>(
            UpdateType.PollAnswer, filter, x => x.PollAnswer);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.PreCheckoutQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PreCheckoutQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PreCheckoutQuery>
        => updater.AddScopedUpdateHandler<THandler, PreCheckoutQuery>(
            UpdateType.PreCheckoutQuery, filter, x => x.PreCheckoutQuery);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ShippingQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ShippingQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ShippingQuery>
        => updater.AddScopedUpdateHandler<THandler, ShippingQuery>(
            UpdateType.ShippingQuery, filter, x => x.ShippingQuery);

    // UpdateType.MessageReaction => typeof(MessageReactionUpdated),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.MessageReaction"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<MessageReactionUpdated>
        => updater.AddScopedUpdateHandler<THandler, MessageReactionUpdated>(
            UpdateType.MessageReaction, filter, x => x.MessageReaction);

    // UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.MessageReactionCount"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionCountUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<MessageReactionCountUpdated>
        => updater.AddScopedUpdateHandler<THandler, MessageReactionCountUpdated>(
            UpdateType.MessageReactionCount, filter, x => x.MessageReactionCount);

    // UpdateType.ChatBoost => typeof(ChatBoostUpdated),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ChatBoost"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatBoostUpdated>
        => updater.AddScopedUpdateHandler<THandler, ChatBoostUpdated>(
            UpdateType.ChatBoost, filter, x => x.ChatBoost);

    // UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.RemovedChatBoost"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostRemoved>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatBoostRemoved>
        => updater.AddScopedUpdateHandler<THandler, ChatBoostRemoved>(
            UpdateType.RemovedChatBoost, filter, x => x.RemovedChatBoost);

    // UpdateType.BusinessConnection => typeof(BusinessConnection),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.BusinessConnection"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<BusinessConnection>? filter = default)
        where THandler : AbstractScopedUpdateHandler<BusinessConnection>
        => updater.AddScopedUpdateHandler<THandler, BusinessConnection>(
            UpdateType.BusinessConnection, filter, x => x.BusinessConnection);

    // UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.DeletedBusinessMessages"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<BusinessMessagesDeleted>? filter = default)
        where THandler : AbstractScopedUpdateHandler<BusinessMessagesDeleted>
        => updater.AddScopedUpdateHandler<THandler, BusinessMessagesDeleted>(
            UpdateType.DeletedBusinessMessages, filter, x => x.DeletedBusinessMessages);

    // UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.PurchasedPaidMedia"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PaidMediaPurchased>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PaidMediaPurchased>
        => updater.AddScopedUpdateHandler<THandler, PaidMediaPurchased>(
            UpdateType.PurchasedPaidMedia, filter, x => x.PurchasedPaidMedia);
}
