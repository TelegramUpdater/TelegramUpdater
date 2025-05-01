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
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler<T>(
        this IUpdater updater,
        UpdateType updateType,
        Func<Update, T?> updateSelector,
        Func<IContainer<T>, Task> callback,
        UpdaterFilter<T>? filter = default,
        int group = 0)
        where T : class
    {
        return updater.AddSingletonUpdateHandler(
            updateHandler: new DefaultHandler<T>(
                updateType: updateType,
                getT: updateSelector,
                callback: callback,
                filter: filter), group);
    }

    /// <summary>
    /// Adds a singleton update handler for <typeparamref name="TUpdate"/>.
    /// </summary>
    /// <typeparam name="TUpdate"></typeparam>
    /// <param name="updater"></param>
    /// <param name="updateType"></param>
    /// <param name="callback"></param>
    /// <param name="filter"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static IUpdater AddSingletonUpdateHandler<TUpdate>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<TUpdate>, Task> callback,
        UpdaterFilter<TUpdate>? filter = default,
        int group = default) where TUpdate : class
        => updater.AddSingletonUpdateHandler(
            updateHandler: new DefaultHandler<TUpdate>(
                updateType: updateType, callback: callback, filter: filter), group);

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
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        UpdateType updateType,
        Func<MessageContainer, Task> callback,
        UpdaterFilter<Message>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            updateType switch
            {
                UpdateType.Message
                    => new MessageHandler(callback, filter),
                UpdateType.EditedMessage
                    => new EditedMessageHandler(callback, filter),
                UpdateType.ChannelPost
                    => new ChannelPostHandler(callback, filter),
                UpdateType.EditedChannelPost
                    => new EditedChannelPostHandler(callback, filter),
                UpdateType.BusinessMessage
                    => new BusinessMessageHandler(callback, filter),
                UpdateType.EditedBusinessMessage
                    => new EditedBusinessMessageHandler(callback, filter),

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a Message.",
                    nameof(updateType)
                ),
            }, group);

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
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<ChatMemberUpdated>, Task> callback,
        UpdaterFilter<ChatMemberUpdated>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            updateType switch
            {
                UpdateType.ChatMember
                    => new ChatMemberHandler(callback, filter ),
                UpdateType.MyChatMember
                    => new MyChatMemberHandler(callback, filter ),

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated.", nameof(updateType)
                ),
            }, group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<CallbackQueryContainer, Task> callback,
        UpdaterFilter<CallbackQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new CallbackQueryHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.InlineQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<InlineQuery>, Task> callback,
        UpdaterFilter<InlineQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new InlineQueryHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChosenInlineResult"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChosenInlineResult>, Task> callback,
        UpdaterFilter<ChosenInlineResult>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ChosenInlineResultHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChatJoinRequest"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatJoinRequest>, Task> callback,
        UpdaterFilter<ChatJoinRequest>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ChatJoinRequestHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.Poll"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<Poll>, Task> callback,
        UpdaterFilter<Poll>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PollHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PollAnswer"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PollAnswer>, Task> callback,
        UpdaterFilter<PollAnswer>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PollAnswerHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PreCheckoutQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PreCheckoutQuery>, Task> callback,
        UpdaterFilter<PreCheckoutQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PreCheckoutQueryHandler(callback, filter), group);

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ShippingQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ShippingQuery>, Task> callback,
        UpdaterFilter<ShippingQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ShippingQueryHandler(callback, filter), group);

    // UpdateType.MessageReaction => typeof(MessageReactionUpdated)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.MessageReaction"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<MessageReactionUpdated>, Task> callback,
        UpdaterFilter<MessageReactionUpdated>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new MessageReactionHandler(callback, filter), group);

    // UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.MessageReactionCount"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<MessageReactionCountUpdated>, Task> callback,
        UpdaterFilter<MessageReactionCountUpdated>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new MessageReactionCountHandler(callback, filter), group);

    // UpdateType.ChatBoost => typeof(ChatBoostUpdated)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChatBoost"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatBoostUpdated>, Task> callback,
        UpdaterFilter<ChatBoostUpdated>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ChatBoostHandler(callback, filter), group);

    // UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.RemovedChatBoost"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatBoostRemoved>, Task> callback,
        UpdaterFilter<ChatBoostRemoved>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new RemovedChatBoostHandler(callback, filter), group);

    // UpdateType.BusinessConnection => typeof(BusinessConnection)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.BusinessConnection"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<BusinessConnection>, Task> callback,
        UpdaterFilter<BusinessConnection>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new BusinessConnectionHandler(callback, filter), group);

    // UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.DeletedBusinessMessages"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<BusinessMessagesDeleted>, Task> callback,
        UpdaterFilter<BusinessMessagesDeleted>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new DeletedBusinessMessagesHandler(callback, filter), group);

    // UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased)
    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PurchasedPaidMedia"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PaidMediaPurchased>, Task> callback,
        UpdaterFilter<PaidMediaPurchased>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PurchasedPaidMediaHandler(callback, filter), group);

}
