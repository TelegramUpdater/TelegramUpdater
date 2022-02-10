using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater.ExceptionHandlers
{
    /// <summary>
    /// Can be used to handle <see cref="Exception"/> of specified type (<typeparamref name="T"/>) in <see cref="Updater"/>.
    /// </summary>
    /// <typeparam name="T">Your <see cref="Exception"/> type.</typeparam>
    public class ExceptionHandler<T> : IExceptionHandler where T : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExceptionHandler{T}"/>
        /// where <typeparamref name="T"/> is a <see cref="Exception"/>
        /// </summary>
        /// <param name="callback">A callback function that will be called when the error catched.</param>
        /// <param name="messageMatch">Handle only when <see cref="Exception.Message"/> matches a text.</param>
        /// <param name="allowedHandlers">
        /// Handle only when the <see cref="Exception"/> occured in specified
        /// <see cref="IUpdateHandler"/>s
        /// <para>Leave null to handle all.</para>
        /// </param>
        /// <exception cref="InvalidCastException">If any of <paramref name="allowedHandlers"/> are not <see cref="IUpdateHandler"/></exception>
        public ExceptionHandler(
            Func<Updater, Exception, Task> callback,
            Filter<string>? messageMatch = default,
            Type[]? allowedHandlers = null,
            bool inherit = false)
        {
            MessageMatch = messageMatch;
            ExceptionType = typeof(T);
            Callback = callback;
            Inherit = inherit;

            if (allowedHandlers != null)
            {
                foreach (var allowedHandler in allowedHandlers)
                {
                    if (!typeof(IUpdateHandler).IsAssignableFrom(allowedHandler))
                    {
                        throw new InvalidCastException($"{allowedHandler} Should be an IUpdateHandler.");
                    }
                }

                AllowedHandlers = allowedHandlers;
            }
        }

        public IEnumerable<Type>? AllowedHandlers { get; }

        public Type ExceptionType { get; }

        public Filter<string>? MessageMatch { get; }

        public Func<Updater, Exception, Task> Callback { get; }

        public bool Inherit { get; }
    }
}
