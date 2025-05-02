using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

namespace TelegramUpdater;

/// <summary>
/// A set of extension methods for <see cref="IUpdater"/> handlers.
/// </summary>
public static class SingletonUpdateHandlerExtensions
{
    /// <summary>
    /// Adds any singleton update handler to the updater.
    /// </summary>
    /// <typeparam name="T">Your update type, Eg: <see cref="Message"/></typeparam>.
    /// <param name="updater"></param>
    /// <param name="updateType"></param>
    /// <param name="updateSelector">
    /// A function to select the right update from <see cref="Update"/>
    /// <para>Eg: <code>update => update.Message</code></para>
    /// </param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public static IUpdater AddSingletonUpdateHandler<T>(
        this IUpdater updater,
        UpdateType updateType,
        Func<Update, T?> updateSelector,
        Func<IContainer<T>, Task> callback,
        UpdaterFilter<T>? filter = default,
        HandlingOptions? options = default)
        where T : class
    {
        return updater.AddSingletonUpdateHandler(
            updateHandler: new DefaultHandler<T>(
                updateType: updateType,
                getT: updateSelector,
                callback: callback,
                filter: filter), options);
    }

    /// <summary>
    /// Adds a singleton update handler for <typeparamref name="TUpdate"/>.
    /// </summary>
    /// <typeparam name="TUpdate"></typeparam>
    /// <param name="updater"></param>
    /// <param name="updateType"></param>
    /// <param name="callback"></param>
    /// <param name="filter"></param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    /// <returns></returns>
    public static IUpdater AddSingletonUpdateHandler<TUpdate>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<TUpdate>, Task> callback,
        UpdaterFilter<TUpdate>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true) where TUpdate : class
        => updater.AddSingletonUpdateHandler(
            updateHandler: new DefaultHandler<TUpdate>(
                updateType: updateType, callback: callback, filter: filter, endpoint: endpoint), options);

    /// <summary>
    /// Adds a handler for any update that is a <see cref="Message"/>.
    /// </summary>
    /// <param name="updateType">
    /// Type of update. should be one of <see cref="UpdateType.Message"/>,
    /// <see cref="UpdateType.EditedMessage"/>, <see cref="UpdateType.ChannelPost"/>,
    /// <see cref="UpdateType.EditedChannelPost"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        UpdateType updateType,
        Func<MessageContainer, Task> callback,
        UpdaterFilter<Message>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            updateType switch
            {
                UpdateType.Message
                    => new MessageHandler(callback, filter, endpoint),
                UpdateType.EditedMessage
                    => new EditedMessageHandler(callback, filter, endpoint),
                UpdateType.ChannelPost
                    => new ChannelPostHandler(callback, filter, endpoint),
                UpdateType.EditedChannelPost
                    => new EditedChannelPostHandler(callback, filter, endpoint),
                UpdateType.BusinessMessage
                    => new BusinessMessageHandler(callback, filter, endpoint),
                UpdateType.EditedBusinessMessage
                    => new EditedBusinessMessageHandler(callback, filter, endpoint),

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a Message.",
                    nameof(updateType)
                ),
            }, options);

    /// <summary>
    /// Adds a handler for any update that is a <see cref="ChatMemberUpdated"/>.
    /// </summary>
    /// <param name="updateType">
    /// Type of update. should be one of
    /// <see cref="UpdateType.ChatMember"/>, <see cref="UpdateType.MyChatMember"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<ChatMemberUpdated>, Task> callback,
        UpdaterFilter<ChatMemberUpdated>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            updateType switch
            {
                UpdateType.ChatMember
                    => new ChatMemberHandler(callback, filter, endpoint),
                UpdateType.MyChatMember
                    => new MyChatMemberHandler(callback, filter, endpoint),

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated.", nameof(updateType)
                ),
            }, options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<CallbackQueryContainer, Task> callback,
        UpdaterFilter<CallbackQuery>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new CallbackQueryHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.InlineQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<InlineQuery>, Task> callback,
        UpdaterFilter<InlineQuery>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new InlineQueryHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChosenInlineResult"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChosenInlineResult>, Task> callback,
        UpdaterFilter<ChosenInlineResult>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new ChosenInlineResultHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChatJoinRequest"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatJoinRequest>, Task> callback,
        UpdaterFilter<ChatJoinRequest>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new ChatJoinRequestHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.Poll"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<Poll>, Task> callback,
        UpdaterFilter<Poll>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new PollHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PollAnswer"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PollAnswer>, Task> callback,
        UpdaterFilter<PollAnswer>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new PollAnswerHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PreCheckoutQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PreCheckoutQuery>, Task> callback,
        UpdaterFilter<PreCheckoutQuery>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new PreCheckoutQueryHandler(callback, filter, endpoint), options);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ShippingQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ShippingQuery>, Task> callback,
        UpdaterFilter<ShippingQuery>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new ShippingQueryHandler(callback, filter, endpoint), options);

    // UpdateType.MessageReaction => typeof(MessageReactionUpdated)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.MessageReaction"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<MessageReactionUpdated>, Task> callback,
        UpdaterFilter<MessageReactionUpdated>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new MessageReactionHandler(callback, filter, endpoint), options);

    // UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.MessageReactionCount"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<MessageReactionCountUpdated>, Task> callback,
        UpdaterFilter<MessageReactionCountUpdated>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new MessageReactionCountHandler(callback, filter, endpoint), options);

    // UpdateType.ChatBoost => typeof(ChatBoostUpdated)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChatBoost"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatBoostUpdated>, Task> callback,
        UpdaterFilter<ChatBoostUpdated>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new ChatBoostHandler(callback, filter, endpoint), options);

    // UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.RemovedChatBoost"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatBoostRemoved>, Task> callback,
        UpdaterFilter<ChatBoostRemoved>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new RemovedChatBoostHandler(callback, filter, endpoint), options);

    // UpdateType.BusinessConnection => typeof(BusinessConnection)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.BusinessConnection"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<BusinessConnection>, Task> callback,
        UpdaterFilter<BusinessConnection>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new BusinessConnectionHandler(callback, filter, endpoint), options);

    // UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<BusinessMessagesDeleted>, Task> callback,
        UpdaterFilter<BusinessMessagesDeleted>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new DeletedBusinessMessagesHandler(callback, filter, endpoint), options);

    // UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="options">Options about how a handler should be handled.</param>
    /// <param name="endpoint">Determines if this an endpoint handler.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PaidMediaPurchased>, Task> callback,
        UpdaterFilter<PaidMediaPurchased>? filter = default,
        HandlingOptions? options = default,
        bool endpoint = true)
        => updater.AddSingletonUpdateHandler(
            new PurchasedPaidMediaHandler(callback, filter, endpoint), options);

}
