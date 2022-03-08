using Microsoft.Extensions.Logging;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater;

/// <summary>
/// Extension methods for <see cref="ExceptionHandler{TException}"/>.
/// </summary>
public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// A callback function that will be called when the error catched.
    /// </param>
    /// <param name="messageMatch">
    /// Handle only when <see cref="Exception.Message"/> matches a text.
    /// </param>
    /// <param name="allowedHandlers">
    /// Handle only when the <see cref="Exception"/> occured in specified
    /// <see cref="IUpdateHandler"/>s
    /// <para>Leave null to handle all.</para>
    /// </param>
    /// <param name="inherit">Allow inherited exceptions and not only exact type.</param>
    public static IUpdater AddExceptionHandler<TException>(
        this IUpdater updater,
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        Type[]? allowedHandlers = null,
        bool inherit = false) where TException : Exception
    {
        return updater.AddExceptionHandler(
            new ExceptionHandler<TException>(callback, messageMatch, allowedHandlers, inherit));
    }

    /// <summary>
    /// Add your exception handler to this updater.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// A callback function that will be called when the error catched.
    /// </param>
    /// <param name="messageMatch">
    /// Handle only when <see cref="Exception.Message"/> matches a text.
    /// </param>
    /// <param name="inherit">
    /// Allow inherited exceptions and not only exact type.
    /// </param>
    public static IUpdater AddExceptionHandler<TException, THandler>(
        this IUpdater updater,
        Func<IUpdater, Exception, Task> callback,
        Filter<string>? messageMatch = default,
        bool inherit = false)
        where TException : Exception where THandler : IUpdateHandler
    {
        return updater.AddExceptionHandler<TException>(
            callback, messageMatch, new[] { typeof(THandler) }, inherit);
    }

    /// <summary>
    /// Adds a default exception handler that uses <see cref="IUpdater.Logger"/>
    /// to notify about exceptions occured in update handlers.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="logLevel">Logging level defaults to <see cref="LogLevel.Error"/>.</param>
    /// <returns></returns>
    public static IUpdater AddDefaultExceptionHandler(
        this IUpdater updater, LogLevel? logLevel = default)
    {
        return updater.AddExceptionHandler<Exception>((u, e) =>
        {
            u.Logger.Log(
                logLevel ?? LogLevel.Error,
                exception: e,
                message: "Execption occured in update handlers");
            return Task.CompletedTask;
        }, inherit: true);
    }
}
