using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TUpdate, TContainer>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<TUpdate>>? filter = default,
        Func<Update, TUpdate?>? getT = default,
        int group = default)
        where THandler : AbstractScopedUpdateHandler<TUpdate, TContainer>
        where TUpdate : class
        where TContainer: IContainer<TUpdate>
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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<Message>>? filter = default)
        where THandler : AbstractScopedUpdateHandler<Message, TContainer>
        where TContainer: IContainer<Message>
        => updater.AddScopedUpdateHandler<THandler, Message, TContainer>(
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

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdateType, Filter{UpdaterFilterInputs{Message}}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<Message>>? filter = default)
        where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
        => updater.AddScopedUpdateHandler<THandler, Message, MessageContainer>(updateType, filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<ChatMemberUpdated>>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatMemberUpdated, TContainer>
        where TContainer : IContainer<ChatMemberUpdated>
        => updater.AddScopedUpdateHandler<THandler, ChatMemberUpdated, TContainer>(
            updateType, filter, updateType switch
            {
                UpdateType.ChatMember => x => x.ChatMember,
                UpdateType.MyChatMember => x => x.MyChatMember,

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated.",
                    nameof(updateType)
                ),
            });

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdateType, Filter{UpdaterFilterInputs{ChatMemberUpdated}}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<ChatMemberUpdated>>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatMemberUpdated, DefaultContainer<ChatMemberUpdated>>
        => updater.AddScopedUpdateHandler<THandler, ChatMemberUpdated, DefaultContainer<ChatMemberUpdated>>(updateType, filter);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </para>
    /// <para>Type of container is <typeparamref name="TContainer"/></para>
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<CallbackQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<CallbackQuery, TContainer>
        where TContainer : IContainer<CallbackQuery>
        => updater.AddScopedUpdateHandler<THandler, CallbackQuery, TContainer>(
            UpdateType.CallbackQuery, filter, x => x.CallbackQuery);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{CallbackQuery}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<CallbackQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<CallbackQuery, CallbackQueryContainer>
        => updater.AddScopedUpdateHandler<THandler, CallbackQueryContainer>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<InlineQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<InlineQuery, TContainer>
        where TContainer : IContainer<InlineQuery>
        => updater.AddScopedUpdateHandler<THandler, InlineQuery, TContainer>(
            UpdateType.InlineQuery, filter, x => x.InlineQuery);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{InlineQuery}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<InlineQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<InlineQuery, DefaultContainer<InlineQuery>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<InlineQuery>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<ChatJoinRequest>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatJoinRequest, TContainer>
        where TContainer : IContainer<ChatJoinRequest>
        => updater.AddScopedUpdateHandler<THandler, ChatJoinRequest, TContainer>(
            UpdateType.ChatJoinRequest, filter, x => x.ChatJoinRequest);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{ChatJoinRequest}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatJoinRequest>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatJoinRequest, DefaultContainer<ChatJoinRequest>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<ChatJoinRequest>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<ChosenInlineResult>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChosenInlineResult, TContainer>
        where TContainer : IContainer<ChosenInlineResult>
        => updater.AddScopedUpdateHandler<THandler, ChosenInlineResult, TContainer>(
            UpdateType.ChosenInlineResult, filter, x => x.ChosenInlineResult);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{ChosenInlineResult}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChosenInlineResult>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChosenInlineResult, DefaultContainer<ChosenInlineResult>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<ChosenInlineResult>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<Poll>? filter = default)
        where THandler : AbstractScopedUpdateHandler<Poll, TContainer>
        where TContainer : IContainer<Poll>
        => updater.AddScopedUpdateHandler<THandler, Poll, TContainer>(
            UpdateType.Poll, filter, x => x.Poll);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{Poll}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<Poll>? filter = default)
        where THandler : AbstractScopedUpdateHandler<Poll, DefaultContainer<Poll>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<Poll>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<PollAnswer>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PollAnswer, TContainer>
        where TContainer : IContainer<PollAnswer>
        => updater.AddScopedUpdateHandler<THandler, PollAnswer, TContainer>(
            UpdateType.PollAnswer, filter, x => x.PollAnswer);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{PollAnswer}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PollAnswer>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PollAnswer, DefaultContainer<PollAnswer>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<PollAnswer>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<PreCheckoutQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PreCheckoutQuery, TContainer>
        where TContainer : IContainer<PreCheckoutQuery>
        => updater.AddScopedUpdateHandler<THandler, PreCheckoutQuery, TContainer>(
            UpdateType.PreCheckoutQuery, filter, x => x.PreCheckoutQuery);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{PreCheckoutQuery}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PreCheckoutQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PreCheckoutQuery, DefaultContainer<PreCheckoutQuery>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<PreCheckoutQuery>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<ShippingQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ShippingQuery, TContainer>
        where TContainer : IContainer<ShippingQuery>
        => updater.AddScopedUpdateHandler<THandler, ShippingQuery, TContainer>(
            UpdateType.ShippingQuery, filter, x => x.ShippingQuery);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{ShippingQuery}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ShippingQuery>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ShippingQuery, DefaultContainer<ShippingQuery>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<ShippingQuery>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<MessageReactionUpdated, TContainer>
        where TContainer : IContainer<MessageReactionUpdated>
        => updater.AddScopedUpdateHandler<THandler, MessageReactionUpdated, TContainer>(
            UpdateType.MessageReaction, filter, x => x.MessageReaction);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{MessageReactionUpdated}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<MessageReactionUpdated, DefaultContainer<MessageReactionUpdated>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<MessageReactionUpdated>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionCountUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<MessageReactionCountUpdated, TContainer>
        where TContainer : IContainer<MessageReactionCountUpdated>
        => updater.AddScopedUpdateHandler<THandler, MessageReactionCountUpdated, TContainer>(
            UpdateType.MessageReactionCount, filter, x => x.MessageReactionCount);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{MessageReactionCountUpdated}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionCountUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<MessageReactionCountUpdated, DefaultContainer<MessageReactionCountUpdated>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<MessageReactionCountUpdated>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatBoostUpdated, TContainer>
        where TContainer : IContainer<ChatBoostUpdated>
        => updater.AddScopedUpdateHandler<THandler, ChatBoostUpdated, TContainer>(
            UpdateType.ChatBoost, filter, x => x.ChatBoost);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{ChatBoostUpdated}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostUpdated>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatBoostUpdated, DefaultContainer<ChatBoostUpdated>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<ChatBoostUpdated>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostRemoved>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatBoostRemoved, TContainer>
        where TContainer : IContainer<ChatBoostRemoved>
        => updater.AddScopedUpdateHandler<THandler, ChatBoostRemoved, TContainer>(
            UpdateType.RemovedChatBoost, filter, x => x.RemovedChatBoost);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{ChatBoostRemoved}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostRemoved>? filter = default)
        where THandler : AbstractScopedUpdateHandler<ChatBoostRemoved, DefaultContainer<ChatBoostRemoved>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<ChatBoostRemoved>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<BusinessConnection>? filter = default)
        where THandler : AbstractScopedUpdateHandler<BusinessConnection, TContainer>
        where TContainer : IContainer<BusinessConnection>
        => updater.AddScopedUpdateHandler<THandler, BusinessConnection, TContainer>(
            UpdateType.BusinessConnection, filter, x => x.BusinessConnection);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{BusinessConnection}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<BusinessConnection>? filter = default)
        where THandler : AbstractScopedUpdateHandler<BusinessConnection, DefaultContainer<BusinessConnection>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<BusinessConnection>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<BusinessMessagesDeleted>? filter = default)
        where THandler : AbstractScopedUpdateHandler<BusinessMessagesDeleted, TContainer>
        where TContainer : IContainer<BusinessMessagesDeleted>
        => updater.AddScopedUpdateHandler<THandler, BusinessMessagesDeleted, TContainer>(
            UpdateType.DeletedBusinessMessages, filter, x => x.DeletedBusinessMessages);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{BusinessMessagesDeleted}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<BusinessMessagesDeleted>? filter = default)
        where THandler : AbstractScopedUpdateHandler<BusinessMessagesDeleted, DefaultContainer<BusinessMessagesDeleted>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<BusinessMessagesDeleted>>(filter);

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
    /// <typeparam name="TContainer">Type of the container.</typeparam>
    public static IUpdater AddScopedUpdateHandler<THandler, TContainer>(
        this IUpdater updater,
        UpdaterFilter<PaidMediaPurchased>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PaidMediaPurchased, TContainer>
        where TContainer : IContainer<PaidMediaPurchased>
        => updater.AddScopedUpdateHandler<THandler, PaidMediaPurchased, TContainer>(
            UpdateType.PurchasedPaidMedia, filter, x => x.PurchasedPaidMedia);

    /// <inheritdoc cref="AddScopedUpdateHandler{THandler, TContainer}(IUpdater, UpdaterFilter{PaidMediaPurchased}?)"/>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PaidMediaPurchased>? filter = default)
        where THandler : AbstractScopedUpdateHandler<PaidMediaPurchased, DefaultContainer<PaidMediaPurchased>>
        => updater.AddScopedUpdateHandler<THandler, DefaultContainer<PaidMediaPurchased>>(filter);
}
