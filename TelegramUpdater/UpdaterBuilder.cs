using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;
using TelegramUpdater.UpdateHandlers.SealedHandlers;

namespace TelegramUpdater
{
    /// <summary>
    /// This class helps you build and configure <see cref="Updater"/> step by step!
    /// </summary>
    public sealed class UpdaterBuilder
    {
        private readonly ITelegramBotClient _botClient;
        private Updater? _updater;

        /// <summary>
        /// - <b>Step zero</b>: Create and Add <see cref="ITelegramBotClient"/>.
        /// <para>
        /// <see cref="ITelegramBotClient"/> is the base! and it's needed for every bot request
        /// that you gonna made, that <see cref="Updater"/> gonna made. so we need that.
        /// </para>
        /// <para>
        /// You'll need an <b>API TOKEN</b> to create <see cref="ITelegramBotClient"/>.
        /// See <see href="https://telegrambots.github.io/book/1/quickstart.html"/>. You can pass
        /// an instance of <see cref="TelegramBotClient"/> or just an <see cref="string"/>
        /// representing you api token.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Call <see cref="StepOne"/> when your done here.
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
        /// that you gonna made, that <see cref="Updater"/> gonna made. so we need that.
        /// </para>
        /// <para>
        /// You'll need an <b>API TOKEN</b> to create <see cref="ITelegramBotClient"/>.
        /// See <see href="https://telegrambots.github.io/book/1/quickstart.html"/>. You can pass
        /// an instance of <see cref="TelegramBotClient"/> or just an <see cref="string"/>
        /// representing you api token.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Call <see cref="StepOne"/> when your done here.
        /// </remarks>
        /// <param name="apiToken">Your bot api token.</param>
        public UpdaterBuilder(string apiToken)
        {
            _botClient = new TelegramBotClient(apiToken);
        }

        /// <summary>
        /// - <b>Step one</b>: Setup updater options using <see cref="UpdaterOptions"/>
        /// <para>
        /// There're several options that you can configure for <see cref="Updater"/>.
        /// If you're not sure or you're not in mode just call it with no inputs.
        /// </para>
        /// </summary>
        /// <param name="maxDegreeOfParallelism">
        /// This a rate controller option! In <see cref="Updater"/>
        /// The updates are handled in parallel, so you have to limit how many updates
        /// can go for handling, at the same time. Defaults to <see cref="Environment.ProcessorCount"/>
        /// </param>
        /// <param name="perUserOneByOneProcess">
        /// This a rate controller option too! By enabling this the <see cref="Updater"/>
        /// won't allow a single user to have more than one update in process at the same time.
        /// The user should wait for the current request of him to finish and has no effects on other users.
        /// </param>
        /// <param name="logger">
        /// This a logger which logs things when it's required. You can use your own customize logger
        /// Or just leave it empty and let us decide.
        /// </param>
        /// <param name="cancellationToken">
        /// This is a token which you can use later to cancel <see cref="Updater.Start(bool, bool, CancellationToken)"/> method
        /// and shut things down.
        /// </param>
        /// <param name="flushUpdatesQueue">
        /// By enabling this, old updates that came when the bot was offline will be ignored!
        /// And updater will start from updates that come since now.
        /// </param>
        /// <param name="allowedUpdates">
        /// A collection of <see cref="UpdateType"/> that you wanna receive.
        /// <para>
        /// Eg: <code>new[] { UpdateType.Message }</code> causes the updater to receive only <see cref="Message"/>s
        /// </para>
        /// </param>
        public UpdaterBuilder StepOne(int? maxDegreeOfParallelism = default,
                                      bool perUserOneByOneProcess = true,
                                      ILogger<Updater>? logger = default,
                                      CancellationToken cancellationToken = default,
                                      bool flushUpdatesQueue = false,
                                      UpdateType[]? allowedUpdates = default)
        {
            var updaterOptions = new UpdaterOptions(maxDegreeOfParallelism,
                                                    perUserOneByOneProcess,
                                                    logger,
                                                    cancellationToken,
                                                    flushUpdatesQueue,
                                                    allowedUpdates);
            _updater = new Updater(_botClient, updaterOptions);
            return this;
        }

        /// <summary>
        /// - <b>Step one</b>: Setup updater options using <see cref="UpdaterOptions"/>
        /// <para>
        /// There're several options that you can configure for <see cref="Updater"/>.
        /// If you're not sure or you're not in mode just call it with no inputs.
        /// </para>
        /// </summary>
        /// <param name="updaterOptions">
        /// Create an instance of <see cref="UpdaterOptions"/> and pass here.
        /// </param>
        /// <remarks>
        /// Call <see cref="StepTwo"/> when you're done here.
        /// </remarks>
        public UpdaterBuilder StepOne(UpdaterOptions updaterOptions)
        {
            _updater = new Updater(_botClient, updaterOptions);
            return this;
        }

        /// <summary>
        /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
        /// <para>
        /// This is how you can be aware of exceptions occured when handling the updates.
        /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
        /// later using <see cref="Updater.AddExceptionHandler(IExceptionHandler)"/>
        /// </para>
        /// <para>
        /// If you're not sure for now, just leave it empty and i'll add a default
        /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
        /// update handler you'll add next.
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="Exception"/> you wanna handle.</typeparam>
        /// <param name="callback">
        /// A callback function that is called when error occured.
        /// </param>
        /// <param name="messageMatch">
        /// An <see cref="string"/> filter on <see cref="Exception.Message"/>.
        /// Use this if you target the Exceptions with an specified message.
        /// <para>
        /// You can use <see cref="Filters.StringRegex"/> to create your filter.
        /// </para>
        /// </param>
        /// <param name="allowedHandlers">
        /// A list of update handlers type, only exceptions occured in them will be handled.
        /// <para>
        /// Leave empty to catch all update handlers.
        /// </para>
        /// </param>
        /// <remarks>
        /// Go for <see cref="StepThree"/> if you're done here too.
        /// </remarks>
        public UpdaterBuilder StepTwo<T>(Func<Updater, Exception, Task> callback,
                                         Filter<string>? messageMatch = default,
                                         Type[]? allowedHandlers = null) where T : Exception
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

            _updater.AddExceptionHandler<T>(callback, messageMatch, allowedHandlers);
            return this;
        }

        /// <summary>
        /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
        /// <para>
        /// This is how you can be aware of exceptions occured when handling the updates.
        /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
        /// later using <see cref="Updater.AddExceptionHandler(IExceptionHandler)"/>
        /// </para>
        /// <para>
        /// If you're not sure for now, just leave it empty and i'll add a default
        /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
        /// update handler you'll add next.
        /// </para>
        /// </summary>
        /// <typeparam name="TException">Type of <see cref="Exception"/> you wanna handle.</typeparam>
        /// <typeparam name="THandler">Type of your handler.</typeparam>
        /// <param name="callback">
        /// A callback function that is called when error occured.
        /// </param>
        /// <param name="messageMatch">
        /// An <see cref="string"/> filter on <see cref="Exception.Message"/>.
        /// Use this if you target the Exceptions with an specified message.
        /// <para>
        /// You can use <see cref="Filters.StringRegex"/> to create your filter.
        /// </para>
        /// </param>
        /// <remarks>
        /// Go for <see cref="StepThree"/> if you're done here too.
        /// </remarks>
        public UpdaterBuilder StepTwo<TException, THandler>(
            Func<Updater, Exception, Task> callback,
            Filter<string>? messageMatch = default)
            where TException : Exception where THandler : IUpdateHandler
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

            _updater.AddExceptionHandler<TException, THandler>(callback, messageMatch);
            return this;
        }

        /// <summary>
        /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
        /// <para>
        /// This is how you can be aware of exceptions occured when handling the updates.
        /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
        /// later using <see cref="Updater.AddExceptionHandler(IExceptionHandler)"/>
        /// </para>
        /// <para>
        /// If you're not sure for now, just leave it empty and i'll add a default
        /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
        /// update handler you'll add next.
        /// </para>
        /// </summary>
        /// <param name="inherit">
        /// If it's <see cref="true"/>, every object that inherits from <see cref="Exception"/>
        /// Will catched! meaning all exceptions.
        /// <para>
        /// If you have exception handlers for specified exceptions, you better turn this off.
        /// </para>
        /// </param>
        /// <remarks>
        /// Go for <see cref="StepThree"/> if you're done here too.
        /// </remarks>
        public UpdaterBuilder StepTwo(bool inherit = true)
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

            _updater.AddExceptionHandler<Exception>(
                (updater, ex) =>
                {
                    updater.Logger.LogError(exception: ex, message: "Error in handlers!");
                    return Task.CompletedTask;
                }, inherit: inherit);
            return this;
        }

        /// <summary>
        /// - <b>Step two</b>: Add an <see cref="ExceptionHandler{T}"/>
        /// <para>
        /// This is how you can be aware of exceptions occured when handling the updates.
        /// Add an <see cref="ExceptionHandler{T}"/> here and you can add more
        /// later using <see cref="Updater.AddExceptionHandler(IExceptionHandler)"/>
        /// </para>
        /// <para>
        /// If you're not sure for now, just leave it empty and i'll add a default
        /// <see cref="ExceptionHandler{T}"/> which handle exceptions in every
        /// update handler you'll add next.
        /// </para>
        /// </summary>
        /// <param name="exceptionHandler">
        /// Your <see cref="ExceptionHandler{T}"/>.
        /// </param>
        /// <remarks>
        /// Go for <see cref="StepThree"/> if you're done here too.
        /// </remarks>
        public UpdaterBuilder StepTwo(IExceptionHandler exceptionHandler)
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepOne ?");

            _updater.AddExceptionHandler(exceptionHandler);
            return this;
        }

        /// <summary>
        /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
        /// <para>
        /// This is the main part! update handler are where you do what you actually excpect from this lib.
        /// Like answering user message and etc.
        /// </para>
        /// <para>
        /// There're two core things you need to create for an <see cref="IUpdateHandler"/>
        /// <list type="number">
        /// <item>
        /// <term><see cref="Filter{T}"/>s</term>
        /// <description>
        /// Filters are used to let <see cref="Updater"/> know what kind of updates you excpect for your handler.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Callback functions</term>
        /// <description>
        /// After an update verified and passed the filters, it's time to handle it.
        /// Handling updates are done in callback functions.
        /// <para>
        /// Callback function gives you an instance of <see cref="UpdateContainerAbs{T}"/>
        /// as argument. and this is all you need! with a large set of Extension methods.
        /// </para>
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// You can use <see cref="FilterCutify"/> class to create your filter.
        /// </para>
        /// <para>
        /// <see cref="FilterCutify.OnCommand(string[])"/> is a good start to handle commands like
        /// <c>/start</c>.
        /// </para>
        /// <para>
        /// <b>For now</b>, pass a callback function and filter here to handle a <see cref="Message"/>.
        /// </para>
        /// </summary>
        /// <param name="callbak">Your callback function.</param>
        /// <param name="filter">Your filter.</param>
        /// <remarks>
        /// You can use <see cref="Updater.AddUpdateHandler(ISingletonUpdateHandler)"/>
        /// or <see cref="Updater.AddScopedHandler(IScopedHandlerContainer)"/>
        /// later to add more update handler.
        /// </remarks>
        public Updater StepThree(
            Func<IContainer<Message>, Task> callbak,
            Filter<Message>? filter)
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

            _updater.AddUpdateHandler(new MessageHandler(callbak, filter));
            return this._updater;
        }

        /// <summary>
        /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
        /// <para>
        /// This is the main part! update handler are where you do what you actually excpect from this lib.
        /// Like answering user message and etc.
        /// </para>
        /// <para>
        /// There're two core things you need to create for an <see cref="IUpdateHandler"/>
        /// <list type="number">
        /// <item>
        /// <term><see cref="Filter{T}"/>s</term>
        /// <description>
        /// Filters are used to let <see cref="Updater"/> know what kind of updates you excpect for your handler.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Callback functions</term>
        /// <description>
        /// After an update verified and passed the filters, it's time to handle it.
        /// Handling updates are done in callback functions.
        /// <para>
        /// Callback function gives you an instance of <see cref="UpdateContainerAbs{T}"/>
        /// as argument. and this is all you need! with a large set of Extension methods.
        /// </para>
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// You can use <see cref="FilterCutify"/> class to create your filter.
        /// </para>
        /// <para>
        /// <see cref="FilterCutify.OnCommand(string[])"/> is a good start to handle commands like
        /// <c>/start</c>.
        /// </para>
        /// <para>
        /// <b>For now</b>, pass a callback function and filter here to handle a <see cref="Message"/>.
        /// </para>
        /// </summary>
        /// <param name="singletonUpdateHandler">
        /// Use classes like <see cref="MessageHandler"/> to create a message handler and such.
        /// </param>
        /// <remarks>
        /// You can use <see cref="Updater.AddUpdateHandler(ISingletonUpdateHandler)"/>
        /// or <see cref="Updater.AddScopedHandler(IScopedHandlerContainer)"/>
        /// later to add more update handler.
        /// </remarks>
        public Updater StepThree(ISingletonUpdateHandler singletonUpdateHandler)
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

            _updater.AddUpdateHandler(singletonUpdateHandler);
            return this._updater;
        }

        /// <summary>
        /// - <b>Step three</b>: Add an <see cref="IUpdateHandler"/>
        /// <para>
        /// This is the main part! update handler are where you do what you actually excpect from this lib.
        /// Like answering user message and etc.
        /// </para>
        /// <para>
        /// There're two core things you need to create for an <see cref="IUpdateHandler"/>
        /// <list type="number">
        /// <item>
        /// <term><see cref="Filter{T}"/>s</term>
        /// <description>
        /// Filters are used to let <see cref="Updater"/> know what kind of updates you excpect for your handler.
        /// </description>
        /// </item>
        /// <item>
        /// <term>Callback functions</term>
        /// <description>
        /// After an update verified and passed the filters, it's time to handle it.
        /// Handling updates are done in callback functions.
        /// <para>
        /// Callback function gives you an instance of <see cref="UpdateContainerAbs{T}"/>
        /// as argument. and this is all you need! with a large set of Extension methods.
        /// </para>
        /// </description>
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// You can use <see cref="FilterCutify"/> class to create your filter.
        /// </para>
        /// <para>
        /// <see cref="FilterCutify.OnCommand(string[])"/> is a good start to handle commands like
        /// <c>/start</c>.
        /// </para>
        /// <para>
        /// <b>For now</b>, pass a callback function and filter here to handle a <see cref="Message"/>.
        /// </para>
        /// </summary>
        /// <param name="scopedHandlerContainer">
        /// Use classes like <see cref="ScopedMessageHandler"/> to create an
        /// scoped message handler and such. Scoped handlers create a new instance of
        /// their underlying handler per each request.
        /// </param>
        /// <remarks>
        /// You can use <see cref="Updater.AddUpdateHandler(ISingletonUpdateHandler)"/>
        /// or <see cref="Updater.AddScopedHandler(IScopedHandlerContainer)"/>
        /// later to add more update handler.
        /// </remarks>
        public Updater StepThree(IScopedHandlerContainer scopedHandlerContainer)
        {
            if (_updater == null)
                throw new InvalidOperationException("Please go step by step, you missed StepTwo ?");

            _updater.AddScopedHandler(scopedHandlerContainer);
            return this._updater;
        }
    }
}
