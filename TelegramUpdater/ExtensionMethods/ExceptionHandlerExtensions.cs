using System;
using System.Threading.Tasks;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater
{
    public static class ExceptionHandlerExtensions
    {
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
        public static Updater AddExceptionHandler<TException>(
            this Updater updater,
            Func<Updater, Exception, Task> callback,
            Filter<string>? messageMatch = default,
            Type[]? allowedHandlers = null) where TException : Exception
        {
            return updater.AddExceptionHandler(
                new ExceptionHandler<TException>(callback, messageMatch, allowedHandlers));
        }

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="callback">A callback function that will be called when the error catched.</param>
        /// <param name="messageMatch">Handle only when <see cref="Exception.Message"/> matches a text.</param>
        public static Updater AddExceptionHandler<TException, THandler>(
            this Updater updater,
            Func<Updater, Exception, Task> callback,
            Filter<string>? messageMatch = default)
            where TException: Exception where THandler: IUpdateHandler
        {
            return updater.AddExceptionHandler<TException>(
                callback, messageMatch, new[] { typeof(THandler) });
        }
    }
}
