﻿// Ignore Spelling: api

using Microsoft.Extensions.Logging;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater;

/// <summary>
/// This class helps you build and configure <see cref="Updater"/> step by step!
/// </summary>
public sealed class UpdaterBuilder
{
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    /// - <b>Step zero</b>: Create and Add <see cref="ITelegramBotClient"/>.
    /// <para>
    /// <see cref="ITelegramBotClient"/> is the base! and it's needed for every bot request
    /// that you gonna made, that <see cref="IUpdater"/> gonna made. so we need that.
    /// </para>
    /// <para>
    /// You'll need an <b>API TOKEN</b> to create <see cref="ITelegramBotClient"/>.
    /// See <see href="https://telegrambots.github.io/book/1/quickstart.html"/>. You can pass
    /// an instance of <see cref="TelegramBotClient"/> or just an <see cref="string"/>
    /// representing you api token.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Call <see cref="StepOne(UpdaterOptions)"/> when your done here.
    /// </remarks>
    /// <param name="botClient">
    /// Create an instance of <see cref="TelegramBotClient"/> and pass here.
    /// </param>
    public UpdaterBuilder(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    /// <summary>
    /// - <b>Step zero</b>: Create and Add <see cref="ITelegramBotClient"/>.
    /// <para>
    /// <see cref="ITelegramBotClient"/> is the base! and it's needed for every bot request
    /// that you gonna made, that <see cref="IUpdater"/> gonna made. so we need that.
    /// </para>
    /// <para>
    /// You'll need an <b>API TOKEN</b> to create <see cref="ITelegramBotClient"/>.
    /// See <see href="https://telegrambots.github.io/book/1/quickstart.html"/>. You can pass
    /// an instance of <see cref="TelegramBotClient"/> or just an <see cref="string"/>
    /// representing you api token.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Call <see cref="StepOne(UpdaterOptions)"/> when your done here.
    /// </remarks>
    /// <param name="apiToken">Your bot api token.</param>
    public UpdaterBuilder(string apiToken)
    {
        _botClient = new TelegramBotClient(apiToken);
    }

    /// <summary>
    /// - <b>Step one</b>: Setup updater options using <see cref="UpdaterOptions"/>
    /// <para>
    /// There are several options that you can configure for <see cref="Updater"/>.
    /// If you're not sure or you're not in mode just call it with no inputs.
    /// </para>
    /// </summary>
    /// <param name="maxDegreeOfParallelism">
    /// This a rate controller option! In <see cref="Updater"/>
    /// The updates are handled in parallel, so you have to limit how many updates
    /// can go for handling, at the same time. Defaults to <see cref="Environment.ProcessorCount"/>
    /// </param>
    /// <param name="logger">
    /// This a logger which logs things when it's required. You can use your own customize logger
    /// Or just leave it empty and let us decide.
    /// </param>
    /// <param name="cancellationToken">
    /// This is a token which you can use later to cancel <see cref="IUpdater.Start(CancellationToken)"/> method
    /// and shut things down.
    /// </param>
    /// <param name="flushUpdatesQueue">
    /// By enabling this, old updates that came when the bot was off will be ignored!
    /// And updater will start from updates that come since now.
    /// </param>
    /// <param name="allowedUpdates">
    /// A collection of <see cref="UpdateType"/> that you wanna receive.
    /// <para>
    /// Eg: <code>new[] { UpdateType.Message }</code> causes the updater to receive only <see cref="Message"/>s
    /// </para>
    /// </param>
    /// <remarks>
    /// Call <see cref="UpdaterBuilderStep2.StepTwo(bool)"/> when you're done here.
    /// </remarks>
    public UpdaterBuilderStep2 StepOne(
        int? maxDegreeOfParallelism = default,
        ILogger<Updater>? logger = default,
        bool flushUpdatesQueue = false,
        UpdateType[]? allowedUpdates = default,
        CancellationToken cancellationToken = default)
    {
        var updaterOptions = new UpdaterOptions(
            maxDegreeOfParallelism: maxDegreeOfParallelism,
            logger: logger,                  
            flushUpdatesQueue: flushUpdatesQueue,
            allowedUpdates: allowedUpdates,
            cancellationToken: cancellationToken);
        return new(new Updater(_botClient, updaterOptions));
    }

    /// <summary>
    /// - <b>Step one</b>: Setup updater options using <see cref="UpdaterOptions"/>
    /// <para>
    /// There are several options that you can configure for <see cref="IUpdater"/>.
    /// If you're not sure or you're not in mode just call it with no inputs.
    /// </para>
    /// </summary>
    /// <param name="updaterOptions">
    /// Create an instance of <see cref="UpdaterOptions"/> and pass here.
    /// </param>
    /// <remarks>
    /// Call <see cref="UpdaterBuilderStep2.StepTwo(bool)"/> when you're done here.
    /// </remarks>
    public UpdaterBuilderStep2 StepOne(UpdaterOptions updaterOptions)
        => new(new Updater(_botClient, updaterOptions));
}

/// <summary>
/// This class helps you build and configure <see cref="Updater"/> step by step!
/// </summary>
/// <remarks>You're now on step 2.</remarks>
public sealed class UpdaterBuilderStep2
{
    private readonly Updater? _updater;

    internal UpdaterBuilderStep2(Updater? updater)
    {
        _updater = updater;
    }

    /// <summary>
    /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
    /// <para>
    /// This is how you can be aware of exceptions occurred when handling the updates.
    /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
    /// later using <see cref="IUpdater.AddExceptionHandler(IExceptionHandler)"/>
    /// </para>
    /// <para>
    /// If you're not sure for now, just leave it empty and I'll add a default
    /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
    /// update handler you'll add next.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of <see cref="Exception"/> you wanna handle.</typeparam>
    /// <param name="callback">
    /// A callback function that is called when error occurred.
    /// </param>
    /// <param name="messageMatch">
    /// An <see cref="string"/> filter on <see cref="Exception.Message"/>.
    /// Use this if you target the Exceptions with an specified message.
    /// <para>
    /// You can use <see cref="Filters.UpdaterStringRegex"/> to create your filter.
    /// </para>
    /// </param>
    /// <param name="allowedHandlers">
    /// A list of update handlers type, only exceptions occurred in them will be handled.
    /// <para>
    /// Leave empty to catch all update handlers.
    /// </para>
    /// </param>
    /// <remarks>
    /// Go for <see cref="UpdaterBuilderStep3.StepThree(IScopedUpdateHandlerContainer)"/> if you're done here too.
    /// </remarks>
    public UpdaterBuilderStep3 StepTwo<T>(
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        Type[]? allowedHandlers = null) where T : Exception
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

        _updater.AddExceptionHandler<T>(callback, messageMatch, allowedHandlers);
        return new(_updater);
    }

    /// <summary>
    /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
    /// <para>
    /// This is how you can be aware of exceptions occurred when handling the updates.
    /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
    /// later using <see cref="IUpdater.AddExceptionHandler(IExceptionHandler)"/>
    /// </para>
    /// <para>
    /// If you're not sure for now, just leave it empty and I'll add a default
    /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
    /// update handler you'll add next.
    /// </para>
    /// </summary>
    /// <typeparam name="TException">Type of <see cref="Exception"/> you wanna handle.</typeparam>
    /// <typeparam name="THandler">Type of your handler.</typeparam>
    /// <param name="callback">
    /// A callback function that is called when error occurred.
    /// </param>
    /// <param name="messageMatch">
    /// An <see cref="string"/> filter on <see cref="Exception.Message"/>.
    /// Use this if you target the Exceptions with an specified message.
    /// <para>
    /// You can use <see cref="Filters.UpdaterStringRegex"/> to create your filter.
    /// </para>
    /// </param>
    /// <remarks>
    /// Go for <see cref="UpdaterBuilderStep3.StepThree(IScopedUpdateHandlerContainer)"/> if you're done here too.
    /// </remarks>
    public UpdaterBuilderStep3 StepTwo<TException, THandler>(
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default)
        where TException : Exception where THandler : IUpdateHandler
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

        _updater.AddExceptionHandler<TException, THandler>(callback, messageMatch);
        return new(_updater);
    }

    /// <summary>
    /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
    /// <para>
    /// This is how you can be aware of exceptions occurred when handling the updates.
    /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
    /// later using <see cref="IUpdater.AddExceptionHandler(IExceptionHandler)"/>
    /// </para>
    /// <para>
    /// If you're not sure for now, just leave it empty and I'll add a default
    /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
    /// update handler you'll add next.
    /// </para>
    /// </summary>
    /// <param name="inherit">
    /// If it's true, every object that inherits from <see cref="Exception"/>
    /// Will catch! meaning all exceptions.
    /// <para>
    /// If you have exception handlers for specified exceptions, you better turn this off.
    /// </para>
    /// </param>
    /// <remarks>
    /// Go for <see cref="UpdaterBuilderStep3.StepThree(IScopedUpdateHandlerContainer)"/> if you're done here too.
    /// </remarks>
    public UpdaterBuilderStep3 StepTwo(bool inherit = true)
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

        _updater.AddExceptionHandler<Exception>(
            (updater, ex) =>
            {
                updater.Logger.LogError(exception: ex, message: "Error in handlers!");
                return Task.CompletedTask;
            }, inherit: inherit);
        return new(_updater);
    }

    /// <summary>
    /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
    /// <para>
    /// This is how you can be aware of exceptions occurred when handling the updates.
    /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
    /// later using <see cref="IUpdater.AddExceptionHandler(IExceptionHandler)"/>
    /// </para>
    /// <para>
    /// If you're not sure for now, just leave it empty and I'll add a default
    /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
    /// update handler you'll add next.
    /// </para>
    /// </summary>
    /// <param name="exceptionHandler">
    /// Your <see cref="ExceptionHandler{T}"/>.
    /// </param>
    /// <remarks>
    /// Go for <see cref="UpdaterBuilderStep3.StepThree(IScopedUpdateHandlerContainer)"/> if you're done here too.
    /// </remarks>
    public UpdaterBuilderStep3 StepTwo(IExceptionHandler exceptionHandler)
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

        _updater.AddExceptionHandler(exceptionHandler);
        return new(_updater);
    }
}

/// <summary>
/// This class helps you build and configure <see cref="Updater"/> step by step!
/// </summary>
/// <remarks>You're now on step 3.</remarks>
public sealed class UpdaterBuilderStep3
{
    private readonly Updater? _updater;

    internal UpdaterBuilderStep3(Updater? updater)
    {
        _updater = updater;
    }

    /// <summary>
    /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
    /// <para>
    /// This is the main part! update handler are where you do what you actually expect from this lib.
    /// Like answering user message and etc.
    /// </para>
    /// <para>
    /// There are two core things you need to create for an <see cref="IUpdateHandler"/>
    /// <list type="number">
    /// <item>
    /// <term><see cref="Filter{T}"/>s</term>
    /// <description>
    /// Filters are used to let <see cref="Updater"/> know what kind of updates you expect for your handler.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Callback functions</term>
    /// <description>
    /// After an update verified and passed the filters, it's time to handle it.
    /// Handling updates are done in callback functions.
    /// <para>
    /// Callback function gives you an instance of <see cref="AbstractUpdateContainer{T}"/>
    /// as argument. and this is all you need! with a large set of Extension methods.
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// You can use <see cref="Filters"/> class to create your filter.
    /// </para>
    /// <para>
    /// <see cref="ReadyFilters.OnCommand(string[])"/> is a good start to handle commands like
    /// <c>/start</c>.
    /// </para>
    /// <para>
    /// <b>For now</b>, pass a callback function and filter here to handle a <see cref="Message"/>.
    /// </para>
    /// </summary>
    /// <param name="callback">Your callback function.</param>
    /// <param name="filter">Your filter.</param>
    /// <remarks>
    /// You can use <see cref="IUpdater.AddSingletonUpdateHandler(ISingletonUpdateHandler, HandlingOptions)"/>
    /// or <see cref="IUpdater.AddScopedUpdateHandler(IScopedUpdateHandlerContainer, HandlingOptions)"/>
    /// later to add more update handler.
    /// You can finally call <see cref="IUpdater.Start{TWriter}(CancellationToken)"/> to fire up your bot.
    /// </remarks>
    public IUpdater StepThree(
        Func<IContainer<Message>, Task> callback,
        UpdaterFilter<Message>? filter)
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

        _updater.AddSingletonUpdateHandler(new UpdateHandlers.Singleton.ReadyToUse.MessageHandler(callback, filter));
        return this._updater;
    }

    /// <summary>
    /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
    /// <para>
    /// This is the main part! update handler are where you do what you actually expect from this lib.
    /// Like answering user message and etc.
    /// </para>
    /// <para>
    /// There are two core things you need to create for an <see cref="IUpdateHandler"/>
    /// <list type="number">
    /// <item>
    /// <term><see cref="Filter{T}"/>s</term>
    /// <description>
    /// Filters are used to let <see cref="Updater"/> know what kind of updates you expect for your handler.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Callback functions</term>
    /// <description>
    /// After an update verified and passed the filters, it's time to handle it.
    /// Handling updates are done in callback functions.
    /// <para>
    /// Callback function gives you an instance of <see cref="AbstractUpdateContainer{T}"/>
    /// as argument. and this is all you need! with a large set of Extension methods.
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// You can use <see cref="Filters"/> class to create your filter.
    /// </para>
    /// <para>
    /// <see cref="ReadyFilters.OnCommand(string[])"/> is a good start to handle commands like
    /// <c>/start</c>.
    /// </para>
    /// <para>
    /// <b>For now</b>, pass a callback function and filter here to handle a <see cref="Message"/>.
    /// </para>
    /// </summary>
    /// <param name="singletonUpdateHandler">
    /// Use classes like <see cref="UpdateHandlers.Singleton.ReadyToUse.MessageHandler"/> to create a message handler and such.
    /// </param>
    /// <remarks>
    /// You can use <see cref="IUpdater.AddSingletonUpdateHandler(ISingletonUpdateHandler, HandlingOptions)"/>
    /// or <see cref="IUpdater.AddScopedUpdateHandler(IScopedUpdateHandlerContainer, HandlingOptions)"/>
    /// later to add more update handler.
    /// You can finally call <see cref="IUpdater.Start{TWriter}(CancellationToken)"/> to fire up your bot.
    /// </remarks>
    public IUpdater StepThree(ISingletonUpdateHandler singletonUpdateHandler)
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

        _updater.AddSingletonUpdateHandler(singletonUpdateHandler);
        return this._updater;
    }

    /// <summary>
    /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
    /// <para>
    /// This is the main part! update handler are where you do what you actually expect from this lib.
    /// Like answering user message and etc.
    /// </para>
    /// <para>
    /// There are two core things you need to create for an <see cref="IUpdateHandler"/>
    /// <list type="number">
    /// <item>
    /// <term><see cref="Filter{T}"/>s</term>
    /// <description>
    /// Filters are used to let <see cref="Updater"/> know what kind of updates you expect for your handler.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Callback functions</term>
    /// <description>
    /// After an update verified and passed the filters, it's time to handle it.
    /// Handling updates are done in callback functions.
    /// <para>
    /// Callback function gives you an instance of <see cref="AbstractUpdateContainer{T}"/>
    /// as argument. and this is all you need! with a large set of Extension methods.
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// You can use <see cref="Filters"/> class to create your filter.
    /// </para>
    /// <para>
    /// <see cref="ReadyFilters.OnCommand(string[])"/> is a good start to handle commands like
    /// <c>/start</c>.
    /// </para>
    /// <para>
    /// <b>For now</b>, pass a callback function and filter here to handle a <see cref="Message"/>.
    /// </para>
    /// </summary>
    /// <param name="scopedHandlerContainer">
    /// Use classes like <see cref="UpdateHandlers.Controller.ReadyToUse.MessageControllerHandler"/> to create an
    /// scoped message handler and such. Scoped handlers create a new instance of
    /// their underlying handler per each request.
    /// </param>
    /// <remarks>
    /// You can use <see cref="IUpdater.AddSingletonUpdateHandler(ISingletonUpdateHandler, HandlingOptions)"/>
    /// or <see cref="IUpdater.AddScopedUpdateHandler(IScopedUpdateHandlerContainer, HandlingOptions)"/>
    /// later to add more update handler.
    /// You can finally call <see cref="IUpdater.Start{TWriter}(CancellationToken)"/> to fire up your bot.
    /// </remarks>
    public IUpdater StepThree(IScopedUpdateHandlerContainer scopedHandlerContainer)
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

        _updater.AddScopedUpdateHandler(scopedHandlerContainer);
        return this._updater;
    }

    /// <summary>
    /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
    /// <para>
    /// This is the main part! update handler are where you do what you actually expect from this lib.
    /// Like answering user message and etc.
    /// </para>
    /// <para>
    /// There are two core things you need to create for an <see cref="IUpdateHandler"/>
    /// <list type="number">
    /// <item>
    /// <term><see cref="Filter{T}"/>s</term>
    /// <description>
    /// Filters are used to let <see cref="Updater"/> know what kind of updates you expect for your handler.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Callback functions</term>
    /// <description>
    /// After an update verified and passed the filters, it's time to handle it.
    /// Handling updates are done in callback functions.
    /// <para>
    /// Callback function gives you an instance of <see cref="AbstractUpdateContainer{T}"/>
    /// as argument. and this is all you need! with a large set of Extension methods.
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// You can use <see cref="Filters"/> class to create your filter.
    /// </para>
    /// <para>
    /// <see cref="ReadyFilters.OnCommand(string[])"/> is a good start to handle commands like
    /// <c>/start</c>.
    /// </para>
    /// </summary>
    /// <param name="autoCollectScopedHandlers">
    /// Set to <see langword="true"/> so that, <see cref="IUpdater"/> will collect scoped handers
    /// automatically ( see <see cref="UpdaterExtensions.CollectHandlers(IUpdater, string)"/> for more info about it ).
    /// otherwise dose nothing to add a handler later.
    /// </param>
    /// <remarks>
    /// You can use <see cref="IUpdater.AddSingletonUpdateHandler(ISingletonUpdateHandler, HandlingOptions)"/>
    /// or <see cref="IUpdater.AddScopedUpdateHandler(IScopedUpdateHandlerContainer, HandlingOptions)"/>
    /// later to add more update handler.
    /// You can finally call <see cref="IUpdater.Start{TWriter}(CancellationToken)"/> to fire up your bot.
    /// </remarks>
    public IUpdater StepThree(bool autoCollectScopedHandlers = false)
    {
        if (_updater == null)
            throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

        if (autoCollectScopedHandlers)
        {
            _updater.CollectHandlers();
        }
        return this._updater;
    }
}
