using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater.Hosting;

/// <summary>
/// Use this class to configure <see cref="IUpdater"/> in an hosting App.
/// </summary>
public class UpdaterServiceBuilder
{
    private readonly List<Type> scopedHandlerTypes;
    private readonly List<Action<IUpdater>> jobs;

    /// <summary>
    /// Creates a new instance of <see cref="UpdaterServiceBuilder"/> class.
    /// </summary>
    public UpdaterServiceBuilder()
    {
        this.scopedHandlerTypes = [];
        this.jobs = [];
    }

    internal void AddToServiceCollection(IServiceCollection serviceDescriptors)
    {
        foreach (var handlerType in scopedHandlerTypes)
        {
            serviceDescriptors.TryAddScoped(handlerType);
        }

        scopedHandlerTypes.Clear();
    }

    internal void ApplyJobs(IUpdater updater)
    {
        foreach (var job in jobs)
        {
            job(updater);
        }
    }

    /// <summary>
    /// Execute any action on the <see cref="IUpdater"/> instance.
    /// </summary>
    /// <remarks>
    /// These actions are only applied on updater, nothing is added to services!
    /// So DON'T use this if you wanna add scoped handlers and they should be
    /// available from services.
    /// </remarks>
    /// <param name="action"></param>
    /// <returns></returns>
    public UpdaterServiceBuilder Execute(Action<IUpdater> action)
    {
        jobs.Add(action);
        return this;
    }

    /// <inheritdoc cref="IUpdater.AddHandler(IScopedUpdateHandlerContainer, HandlingOptions?)"/>
    public UpdaterServiceBuilder AddScopedUpdateHandler(
        IScopedUpdateHandlerContainer scopedHandlerContainer, HandlingOptions? options)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(scopedHandlerContainer);
#else
        if (scopedHandlerContainer is null)
        {
            throw new ArgumentNullException(nameof(scopedHandlerContainer));
        }
#endif

        scopedHandlerTypes.Add(scopedHandlerContainer.ScopedHandlerType);
        return Execute(updater => updater.AddHandler(scopedHandlerContainer, options));
    }

    /// <inheritdoc cref="ScopedUpdateHandlersExtensions.AddScopedUpdateHandler{THandler, TUpdate, TContainer}(IUpdater, UpdateType, Filter{UpdaterFilterInputs{TUpdate}}?, Func{Update, TUpdate?}?, HandlingOptions?)"/>
    public UpdaterServiceBuilder AddHandler<THandler, TUpdate, TContainer>(
        UpdateType updateType,
        UpdaterFilter<TUpdate>? filter = default,
        Func<Update, TUpdate?>? getT = default,
        HandlingOptions? options = default)
        where THandler : AbstractScopedUpdateHandler<TUpdate, TContainer>
        where TUpdate : class
        where TContainer : IContainer<TUpdate>
    {
        scopedHandlerTypes.Add(typeof(THandler));
        return Execute(updater => updater.AddHandler<THandler, TUpdate, TContainer>(updateType, filter, getT, options));
    }


    /// <inheritdoc cref="ScopedUpdateHandlersExtensions.AddHandler{TUpdate}(IUpdater, Type, UpdateType, UpdaterFilter{TUpdate}?, Func{Update, TUpdate}?, HandlingOptions?)"/>
    public UpdaterServiceBuilder AddHandler<TUpdate>(
        Type typeOfScopedHandler,
        UpdateType updateType,
        UpdaterFilter<TUpdate>? filter = default,
        Func<Update, TUpdate>? getT = default,
        HandlingOptions? options = default) where TUpdate : class
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(typeOfScopedHandler);
#else
        if (typeOfScopedHandler is null)
        {
            throw new ArgumentNullException(nameof(typeOfScopedHandler));
        }
#endif

        if (!typeof(IScopedUpdateHandler).IsAssignableFrom(typeOfScopedHandler))
        {
            throw new InvalidCastException(
                $"{typeOfScopedHandler} Should be an IScopedUpdateHandler.");
        }

        scopedHandlerTypes.Add(typeOfScopedHandler);
        return Execute(updater => updater.AddHandler(typeOfScopedHandler, updateType, filter, getT, options));
    }

    /// <inheritdoc cref="IUpdater.AddExceptionHandler(IExceptionHandler)"/>
    public UpdaterServiceBuilder AddExceptionHandler(IExceptionHandler exceptionHandler)
        => Execute(updater => updater.AddExceptionHandler(exceptionHandler));

    /// <inheritdoc cref="ExceptionHandlerExtensions.AddExceptionHandler{TException, THandler}(IUpdater, Func{IUpdater, Exception, Task}, Filter{string}?, bool)"/>
    public UpdaterServiceBuilder AddExceptionHandler<TException>(
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        Type[]? allowedHandlers = null,
        bool inherit = false) where TException : Exception
        => Execute(x => x.AddExceptionHandler(
            new ExceptionHandler<TException>(callback, messageMatch, allowedHandlers, inherit)));

    /// <inheritdoc cref="ExceptionHandlerExtensions.AddExceptionHandler{TException, THandler}(IUpdater, Func{IUpdater, Exception, Task}, Filter{string}?, bool)"/>
    public UpdaterServiceBuilder AddExceptionHandler<TException, THandler>(
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        bool inherit = false)
        where TException : Exception where THandler : IUpdateHandler
        => Execute(updater => updater.AddExceptionHandler<TException>(
            callback, messageMatch, [typeof(THandler)], inherit));

    /// <inheritdoc cref="ExceptionHandlerExtensions.AddDefaultExceptionHandler(IUpdater, LogLevel?)"/>
    public UpdaterServiceBuilder AddDefaultExceptionHandler(LogLevel? logLevel = default)
        => Execute(updater => updater.AddDefaultExceptionHandler(logLevel));

    /// <inheritdoc cref="UpdaterExtensions.CollectHandlers(IUpdater, string)"/>
    public UpdaterServiceBuilder CollectHandlers(
        string handlersParentNamespace = "UpdateHandlers")
    {
        foreach (var (_, _, handlerType) in UpdaterExtensions
            .IterCollectedScopedUpdateHandlerTypes(handlersParentNamespace))
        {
            if (handlerType is null) continue;

            scopedHandlerTypes.Add(handlerType);
        }

        return Execute(updater => updater.CollectHandlers(handlersParentNamespace));
    }

    /// <summary>
    /// Adds an scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.Message"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filler using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddMessageHandler<THandler>(
        UpdaterFilter<Message>? filter = default,
        HandlingOptions? options = default)
        where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
        => AddHandler<THandler, Message, MessageContainer>(
            UpdateType.Message,
            filter,
            x => x.Message,
            options);

    /// <summary>
    /// Adds an scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddCallbackQueryHandler<THandler>(
        UpdaterFilter<CallbackQuery>? filter = default,
        HandlingOptions? options = default)
        where THandler : AbstractScopedUpdateHandler<CallbackQuery, CallbackQueryContainer>
        => AddHandler<THandler, CallbackQuery, CallbackQueryContainer>(
            UpdateType.CallbackQuery,
            filter,
            x => x.CallbackQuery,
            options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.EditedMessage"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddEditedMessageHandler<THandler>(
       UpdaterFilter<Message>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
       => AddHandler<THandler, Message, MessageContainer>(
           UpdateType.EditedMessage,
           filter,
           x => x.EditedMessage,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.ChannelPost"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddChannelPostHandler<THandler>(
       UpdaterFilter<Message>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
       => AddHandler<THandler, Message, MessageContainer>(
           UpdateType.ChannelPost,
           filter,
           x => x.ChannelPost,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.EditedChannelPost"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddEditedChannelPostHandler<THandler>(
       UpdaterFilter<Message>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
       => AddHandler<THandler, Message, MessageContainer>(
           UpdateType.EditedChannelPost,
           filter,
           x => x.EditedChannelPost,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.InlineQuery"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddInlineQueryHandler<THandler>(
       UpdaterFilter<InlineQuery>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<InlineQuery, DefaultContainer<InlineQuery>>
       => AddHandler<THandler, InlineQuery, DefaultContainer<InlineQuery>>(
           UpdateType.InlineQuery,
           filter,
           x => x.InlineQuery,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.ChosenInlineResult"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddChosenInlineResultHandler<THandler>(
       UpdaterFilter<ChosenInlineResult>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<ChosenInlineResult, DefaultContainer<ChosenInlineResult>>
       => AddHandler<THandler, ChosenInlineResult, DefaultContainer<ChosenInlineResult>>(
           UpdateType.ChosenInlineResult,
           filter,
           x => x.ChosenInlineResult,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.ShippingQuery"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddShippingQueryHandler<THandler>(
       UpdaterFilter<ShippingQuery>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<ShippingQuery, DefaultContainer<ShippingQuery>>
       => AddHandler<THandler, ShippingQuery, DefaultContainer<ShippingQuery>>(
           UpdateType.ShippingQuery,
           filter,
           x => x.ShippingQuery,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.PreCheckoutQuery"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddPreCheckoutQueryHandler<THandler>(
       UpdaterFilter<PreCheckoutQuery>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<PreCheckoutQuery, DefaultContainer<PreCheckoutQuery>>
       => AddHandler<THandler, PreCheckoutQuery, DefaultContainer<PreCheckoutQuery>>(
           UpdateType.PreCheckoutQuery,
           filter,
           x => x.PreCheckoutQuery,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.Poll"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddPollHandler<THandler>(
       UpdaterFilter<Poll>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<Poll, DefaultContainer<Poll>>
       => AddHandler<THandler, Poll, DefaultContainer<Poll>>(
           UpdateType.Poll,
           filter,
           x => x.Poll,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.PollAnswer"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddPollAnswerHandler<THandler>(
       UpdaterFilter<PollAnswer>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<PollAnswer, DefaultContainer<PollAnswer>>
       => AddHandler<THandler, PollAnswer, DefaultContainer<PollAnswer>>(
           UpdateType.PollAnswer,
           filter,
           x => x.PollAnswer,
           options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.MyChatMember"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddMyChatMemberHandler<THandler>(
      UpdaterFilter<ChatMemberUpdated>? filter = default,
      HandlingOptions? options = default)
      where THandler : AbstractScopedUpdateHandler<ChatMemberUpdated, DefaultContainer<ChatMemberUpdated>>
      => AddHandler<THandler, ChatMemberUpdated, DefaultContainer<ChatMemberUpdated>>(
          UpdateType.MyChatMember,
          filter,
          x => x.MyChatMember,
          options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.ChatMember"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddChatMemberHandler<THandler>(
      UpdaterFilter<ChatMemberUpdated>? filter = default,
      HandlingOptions? options = default)
      where THandler : AbstractScopedUpdateHandler<ChatMemberUpdated, DefaultContainer<ChatMemberUpdated>>
      => AddHandler<THandler, ChatMemberUpdated, DefaultContainer<ChatMemberUpdated>>(
          UpdateType.ChatMember,
          filter,
          x => x.ChatMember,
          options);

    /// <summary>
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.ChatJoinRequest"/>.
    /// </summary>
    /// <typeparam name="THandler">Your handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    /// <param name="options">Options about how a handler should be handled.</param>
    public UpdaterServiceBuilder AddChatJoinRequestHandler<THandler>(
      UpdaterFilter<ChatJoinRequest>? filter = default,
      HandlingOptions? options = default)
      where THandler : AbstractScopedUpdateHandler<ChatJoinRequest, DefaultContainer<ChatJoinRequest>>
      => AddHandler<THandler, ChatJoinRequest, DefaultContainer<ChatJoinRequest>>(
          UpdateType.ChatJoinRequest,
          filter,
          x => x.ChatJoinRequest,
          options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.MessageReaction"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddMessageReactionHandler<THandler>(
       UpdaterFilter<MessageReactionUpdated>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<MessageReactionUpdated, DefaultContainer<MessageReactionUpdated>>
       => AddHandler<THandler, MessageReactionUpdated, DefaultContainer<MessageReactionUpdated>>(
           UpdateType.MessageReaction,
           filter,
           x => x.MessageReaction,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.MessageReactionCount"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddMessageReactionCountHandler<THandler>(
       UpdaterFilter<MessageReactionCountUpdated>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<MessageReactionCountUpdated, DefaultContainer<MessageReactionCountUpdated>>
       => AddHandler<THandler, MessageReactionCountUpdated, DefaultContainer<MessageReactionCountUpdated>>(
           UpdateType.MessageReactionCount,
           filter,
           x => x.MessageReactionCount,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.ChatBoost"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddChatBoostHandler<THandler>(
       UpdaterFilter<ChatBoostUpdated>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<ChatBoostUpdated, DefaultContainer<ChatBoostUpdated>>
       => AddHandler<THandler, ChatBoostUpdated, DefaultContainer<ChatBoostUpdated>>(
           UpdateType.ChatBoost,
           filter,
           x => x.ChatBoost,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.RemovedChatBoost"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddRemovedChatBoostHandler<THandler>(
       UpdaterFilter<ChatBoostRemoved>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<ChatBoostRemoved, DefaultContainer<ChatBoostRemoved>>
       => AddHandler<THandler, ChatBoostRemoved, DefaultContainer<ChatBoostRemoved>>(
           UpdateType.RemovedChatBoost,
           filter,
           x => x.RemovedChatBoost,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.BusinessConnection"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddBusinessConnectionHandler<THandler>(
       UpdaterFilter<BusinessConnection>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<BusinessConnection, DefaultContainer<BusinessConnection>>
       => AddHandler<THandler, BusinessConnection, DefaultContainer<BusinessConnection>>(
           UpdateType.BusinessConnection,
           filter,
           x => x.BusinessConnection,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.BusinessMessage"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddBusinessMessageHandler<THandler>(
       UpdaterFilter<Message>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
       => AddHandler<THandler, Message, MessageContainer>(
           UpdateType.BusinessMessage,
           filter,
           x => x.BusinessMessage,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.EditedBusinessMessage"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddEditedBusinessMessageHandler<THandler>(
       UpdaterFilter<Message>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<Message, MessageContainer>
       => AddHandler<THandler, Message, MessageContainer>(
           UpdateType.EditedBusinessMessage,
           filter,
           x => x.EditedBusinessMessage,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.DeletedBusinessMessages"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddDeletedBusinessMessagesHandler<THandler>(
       UpdaterFilter<BusinessMessagesDeleted>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<BusinessMessagesDeleted, DefaultContainer<BusinessMessagesDeleted>>
       => AddHandler<THandler, BusinessMessagesDeleted, DefaultContainer<BusinessMessagesDeleted>>(
           UpdateType.DeletedBusinessMessages,
           filter,
           x => x.DeletedBusinessMessages,
           options);

    /// <summary>  
    /// Adds a scoped handler to the <see cref="IUpdater"/>, for updates of type <see cref="UpdateType.PurchasedPaidMedia"/>.  
    /// </summary>  
    /// <typeparam name="THandler">Your handler type.</typeparam>  
    /// <param name="filter">  
    /// The filter to choose the right updates to handle.  
    /// <para>  
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.  
    /// </para>  
    /// </param>  
    /// <param name="options">Options about how a handler should be handled.</param>  
    public UpdaterServiceBuilder AddPurchasedPaidMediaHandler<THandler>(
       UpdaterFilter<PaidMediaPurchased>? filter = default,
       HandlingOptions? options = default)
       where THandler : AbstractScopedUpdateHandler<PaidMediaPurchased, DefaultContainer<PaidMediaPurchased>>
       => AddHandler<THandler, PaidMediaPurchased, DefaultContainer<PaidMediaPurchased>>(
           UpdateType.PurchasedPaidMedia,
           filter,
           x => x.PurchasedPaidMedia,
           options);

    /// <inheritdoc cref="UpdaterExtensions.QuickStartCommandReply(IUpdater, string, bool, ParseMode, IEnumerable{MessageEntity}?, bool?, int?, bool, ReplyMarkup?, bool, string?, string?, bool, bool, CancellationToken)"/>
    public UpdaterServiceBuilder QuickStartCommandReply(
        string text,
        bool sendAsReply = true,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        int? messageThreadId = default,
        bool disableNotification = default,
        ReplyMarkup? replyMarkup = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        bool allowSendingWithoutReply = true)
    {
        return Execute(updater => updater.QuickStartCommandReply(text: text,
                sendAsReply: sendAsReply,
                parseMode: parseMode,
                messageEntities: messageEntities,
                disableWebpagePreview: disableWebpagePreview,
                messageThreadId: messageThreadId,
                disableNotification: disableNotification,
                replyMarkup: replyMarkup,
                protectContent: protectContent,
                messageEffectId: messageEffectId,
                businessConnectionId: businessConnectionId,
                allowPaidBroadcast: allowPaidBroadcast,
                allowSendingWithoutReply: allowSendingWithoutReply, cancellationToken: updater.UpdaterOptions.CancellationToken));
    }
}
