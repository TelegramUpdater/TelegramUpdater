using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater.Hosting;

/// <summary>
/// Use this class to configure <see cref="IUpdater"/> in an hosting app.
/// </summary>
public class UpdaterServiceBuilder
{
    private readonly List<IScopedUpdateHandlerContainer> _scopedHandlerContainers;
    private readonly List<Action<IUpdater>> _otherExecutions;

    /// <summary>
    /// Initialize a new instance of <see cref="UpdaterServiceBuilder"/>.
    /// </summary>
    public UpdaterServiceBuilder()
    {
        _scopedHandlerContainers = new();
        _otherExecutions = new();
    }

    internal void AddToUpdater(IUpdater updater)
    {
        foreach (var container in _scopedHandlerContainers)
        {
            updater.AddScopedUpdateHandler(container);
        }

        foreach (var execution in _otherExecutions)
        {
            execution(updater);
        }

        _scopedHandlerContainers.Clear();
    }

    internal IEnumerable<IScopedUpdateHandlerContainer> IterScopedContainers()
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
            serviceDescriptors.AddScoped(container.ScopedHandlerType);
        }
    }

    /// <summary>
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <param name="scopedHandlerContainer">
    /// Use <see cref="ScopedUpdateHandlerContainerBuilder{THandler, TUpdate}"/>
    /// To Create a new <see cref="IScopedUpdateHandlerContainer"/>
    /// </param>
    public UpdaterServiceBuilder AddScopedUpdateHandler(
        IScopedUpdateHandlerContainer scopedHandlerContainer)
    {
        _scopedHandlerContainers.Add(scopedHandlerContainer);
        return this;
    }

    /// <summary>
    /// Adds an scoped handler to the updater. ( Use this if you'r not sure. )
    /// </summary>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    public UpdaterServiceBuilder AddScopedUpdateHandler<THandler, TUpdate>(
        Filter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate>? getT = default)
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
                updateType.Value, filter, getT));
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
    public UpdaterServiceBuilder AddScopedUpdateHandler<TUpdate>(
        Type typeOfScopedHandler,
        Filter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate>? getT = default) where TUpdate : class
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
            containerGeneric, new object?[] { updateType.Value, filter, getT });

        if (container != null)
        {
            return AddScopedUpdateHandler(container);
        }
        else
        {
            throw new InvalidOperationException(
                "Handler not added to the Scoped Handlers! The instance of it is null.");
        }
    }

    /// <summary>
    /// Adds an scoped <see cref="Message"/> handler to the <see cref="IUpdater"/>.
    /// </summary>
    /// <typeparam name="THandler">Your <see cref="Message"/> handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    public UpdaterServiceBuilder AddMessageHandler<THandler>(Filter<Message>? filter = default)
        where THandler : IScopedUpdateHandler
        => AddScopedUpdateHandler<THandler, Message>(filter, UpdateType.Message, x => x.Message!);

    /// <summary>
    /// Adds an scoped <see cref="CallbackQuery"/> handler to the updater.
    /// </summary>
    /// <typeparam name="THandler">Your <see cref="CallbackQuery"/> handler type.</typeparam>
    /// <param name="filter">
    /// The filter to choose the right updates to handle.
    /// <para>
    /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
    /// </para>
    /// </param>
    public UpdaterServiceBuilder AddCallbackQueryHandler<THandler>(Filter<CallbackQuery>? filter = default)
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
    public UpdaterServiceBuilder ExecuteOthers(Action<IUpdater> action)
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
        ExecuteOthers(updater => updater.AddExceptionHandler(exceptionHandler));
        return this;
    }

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="callback">A callback function that will be called when the error catched.</param>
    /// <param name="messageMatch">Handle only when <see cref="Exception.Message"/> matches a text.</param>
    /// <param name="allowedHandlers">
    /// Handle only when the <see cref="Exception"/> occured in specified
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
        return ExecuteOthers(x => x.AddExceptionHandler(
            new ExceptionHandler<TException>(callback, messageMatch, allowedHandlers, inherit)));
    }

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="callback">
    /// A callback function that will be called when the error catched.
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
        return ExecuteOthers(updater => updater.AddExceptionHandler<TException>(
            callback, messageMatch, new[] { typeof(THandler) }, inherit));
    }

    /// <summary>
    /// Add default exception handler to the updater.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public UpdaterServiceBuilder AddDefaultExceptionHandler(LogLevel? logLevel = default)
    {
        return ExecuteOthers(updater => updater.AddDefaultExceptionHandler(logLevel));
    }

    private static bool TryResovleNamespaceToUpdateType(
        string currentNs, [NotNullWhen(true)] out Type? type)
    {
        var nsParts = currentNs.Split('.');
        if (nsParts.Length < 3)
            throw new Exception("Namespace is invalid.");

        type = nsParts[2] switch
        {
            "Messages" => typeof(Message),
            "CallbackQueries" => typeof(CallbackQuery),
            "InlineQueries" => typeof(InlineQuery),
            _ => null
        };

        if (type is null)
            return false;
        return true;
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
    /// Parent name should match the update type name, eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <paramref name="handlersParentNamespace"/>/Messages/MyScopedMessageHandler
    /// </remarks>
    /// <returns></returns>
    public UpdaterServiceBuilder AutoCollectScopedHandlers(
        string handlersParentNamespace = "UpdateHandlers")
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
            throw new ApplicationException("Can't find entry assembly.");

        var assemplyName = entryAssembly.GetName().Name;

        var handlerNs = $"{assemplyName}.{handlersParentNamespace}";

        // All types in *handlersParentNamespace*
        var scopedHandlersTypes = entryAssembly.GetTypes()
            .Where(x =>
                x.Namespace is not null &&
                x.Namespace.StartsWith(handlerNs))
            .Where(x => x.IsClass)
            .Where(x => typeof(IScopedUpdateHandler).IsAssignableFrom(x));

        foreach (var scopedType in scopedHandlersTypes)
        {
            if (!TryResovleNamespaceToUpdateType(scopedType.Namespace!, out var updateType))
            {
                continue;
            }

            var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>)
                .MakeGenericType(scopedType, updateType);

            var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
                containerGeneric,
                new object?[]
                {
                        Enum.Parse<UpdateType>(updateType.Name), null, null
                });

            if (container is null) continue;

            AddScopedUpdateHandler(container);
        }

        return this;
    }
}
