using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater.Hosting;

/// <summary>
/// Use this class to configure <see cref="IUpdater"/> in an hosting App.
/// </summary>
public class UpdaterServiceBuilder
{
    private readonly List<HandlingInfo<IScopedUpdateHandlerContainer>> _scopedHandlerContainers;
    private readonly List<Action<IUpdater>> _otherExecutions;

    /// <summary>
    /// Initialize a new instance of <see cref="UpdaterServiceBuilder"/>.
    /// </summary>
    public UpdaterServiceBuilder()
    {
        _scopedHandlerContainers = [];
        _otherExecutions = [];
    }

    internal void AddToUpdater(IUpdater updater)
    {
        foreach (var container in _scopedHandlerContainers)
        {
            updater.AddScopedUpdateHandler(container.Handler, container.Group);
        }

        foreach (var execution in _otherExecutions)
        {
            execution(updater);
        }

        _scopedHandlerContainers.Clear();
    }

    internal IEnumerable<HandlingInfo<IScopedUpdateHandlerContainer>> IterScopedContainers()
    {
        foreach (var container in _scopedHandlerContainers)
        {
            yield return container;
        }
    }

    internal void AddToServiceCollection(IServiceCollection serviceDescriptors)
    {
        foreach (var container in IterScopedContainers())
        {
            serviceDescriptors.AddScoped(container.Handler.ScopedHandlerType);
        }
    }

    /// <summary>
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <param name="scopedHandlerContainer">
    /// Use <see cref="ScopedUpdateHandlerContainerBuilder{THandler, TUpdate}"/>
    /// To Create a new <see cref="IScopedUpdateHandlerContainer"/>
    /// </param>
    /// <param name="group"></param>
    public UpdaterServiceBuilder AddScopedUpdateHandler(
        IScopedUpdateHandlerContainer scopedHandlerContainer, int group)
    {
        _scopedHandlerContainers.Add(new(scopedHandlerContainer, group));
        return this;
    }

    /// <summary>
    /// Adds an scoped handler to the updater. ( Use this if you're not sure. )
    /// </summary>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <param name="group"></param>
    public UpdaterServiceBuilder AddScopedUpdateHandler<THandler, TUpdate>(
        UpdaterFilter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate>? getT = default,
        int group = default)
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        if (updateType == null)
        {
            var _t = typeof(TUpdate);
            if (Enum.TryParse(_t.Name, out UpdateType ut))
            {
                updateType = ut;
            }
            else
            {
                throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
            }
        }

        return AddScopedUpdateHandler(
            new ScopedUpdateHandlerContainerBuilder<THandler, TUpdate>(
                updateType.Value, filter, getT), group);
    }

    /// <summary>
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="typeOfScopedHandler">Type of your handler.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <param name="group"></param>
    public UpdaterServiceBuilder AddScopedUpdateHandler<TUpdate>(
        Type typeOfScopedHandler,
        UpdaterFilter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate>? getT = default,
        int group = default) where TUpdate : class
    {
        if (!typeof(IScopedUpdateHandler).IsAssignableFrom(typeOfScopedHandler))
        {
            throw new InvalidCastException($"{typeOfScopedHandler} Should be an IScopedUpdateHandler");
        }

        var _t = typeof(TUpdate);
        if (updateType == null)
        {
            if (Enum.TryParse(_t.Name, out UpdateType ut))
            {
                updateType = ut;
            }
            else
            {
                throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
            }
        }

        var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>)
            .MakeGenericType(typeOfScopedHandler, _t);

        var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
            containerGeneric, [updateType.Value, filter, getT]);

        if (container != null)
        {
            return AddScopedUpdateHandler(container, group);
        }

        throw new InvalidOperationException(
            "Handler not added to the Scoped Handlers! The instance of it is null.");
    }

    /// <summary>
    /// Adds an scoped <see cref="Message"/> handler to the <see cref="IUpdater"/>.
    /// </summary>
    /// <typeparam name="THandler">Your <see cref="Message"/> handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filler using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    public UpdaterServiceBuilder AddMessageHandler<THandler>(UpdaterFilter<Message>? filter = default)
        where THandler : IScopedUpdateHandler
        => AddScopedUpdateHandler<THandler, Message>(filter, UpdateType.Message, x => x.Message!);

    /// <summary>
    /// Adds an scoped <see cref="CallbackQuery"/> handler to the updater.
    /// </summary>
    /// <typeparam name="THandler">Your <see cref="CallbackQuery"/> handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your filter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    public UpdaterServiceBuilder AddCallbackQueryHandler<THandler>(UpdaterFilter<CallbackQuery>? filter = default)
        where THandler : IScopedUpdateHandler
        => AddScopedUpdateHandler<THandler, CallbackQuery>(filter, UpdateType.Message, x => x.CallbackQuery!);

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
        _otherExecutions.Add(action);
        return this;
    }

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="exceptionHandler"></param>
    public UpdaterServiceBuilder AddExceptionHandler(IExceptionHandler exceptionHandler)
    {
        Execute(updater => updater.AddExceptionHandler(exceptionHandler));
        return this;
    }

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="callback">A callback function that will be called when the error catches.</param>
    /// <param name="messageMatch">Handle only when <see cref="Exception.Message"/> matches a text.</param>
    /// <param name="allowedHandlers">
    /// Handle only when the <see cref="Exception"/> occurred in specified
    /// <see cref="IUpdateHandler"/>s
    /// <para>Leave null to handle all.</para>
    /// </param>
    /// <param name="inherit">
    /// Indicates if this handler should catch all of exceptions
    /// that are inherited from <typeparamref name="TException"/>.
    /// </param>
    public UpdaterServiceBuilder AddExceptionHandler<TException>(
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        Type[]? allowedHandlers = null,
        bool inherit = false) where TException : Exception
    {
        return Execute(x => x.AddExceptionHandler(
            new ExceptionHandler<TException>(callback, messageMatch, allowedHandlers, inherit)));
    }

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="callback">
    /// A callback function that will be called when the error catches.
    /// </param>
    /// <param name="messageMatch">
    /// Handle only when <see cref="Exception.Message"/> matches a text.
    /// </param>
    /// <param name="inherit">
    /// Indicates if this handler should catch all of exceptions
    /// that are inherited from <typeparamref name="TException"/>.
    /// </param>
    public UpdaterServiceBuilder AddExceptionHandler<TException, THandler>(
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        bool inherit = false)
        where TException : Exception where THandler : IUpdateHandler
    {
        return Execute(updater => updater.AddExceptionHandler<TException>(
            callback, messageMatch, [typeof(THandler)], inherit));
    }

    /// <summary>
    /// Add default exception handler to the updater.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public UpdaterServiceBuilder AddDefaultExceptionHandler(LogLevel? logLevel = default)
    {
        return Execute(updater => updater.AddDefaultExceptionHandler(logLevel));
    }

    /// <summary>
    /// Automatically collects all classes that are marked as scoped handlers
    /// And adds them to the <see cref="IUpdater"/> instance.
    /// </summary>
    /// <remarks>
    /// <b>Considerations:</b>
    /// <list type="number">
    /// <item>
    /// You should place handlers of different update types
    /// ( <see cref="UpdateType.Message"/>, <see cref="UpdateType.CallbackQuery"/> and etc. )
    /// into different parent folders.
    /// </item>
    /// <item>
    /// Parent name should match the update type name, Eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <paramref name="handlersParentNamespace"/>/Messages/MyScopedMessageHandler
    /// </remarks>
    /// <returns></returns>
    public UpdaterServiceBuilder AutoCollectScopedHandlers(
        string handlersParentNamespace = "UpdateHandlers")
    {
        foreach (var container in UpdaterExtensions
            .IterCollectedScopedContainers(handlersParentNamespace))
        {
            if (container is null) continue;

            AddScopedUpdateHandler(container.Handler, container.Group);
        }

        return this;
    }
}
