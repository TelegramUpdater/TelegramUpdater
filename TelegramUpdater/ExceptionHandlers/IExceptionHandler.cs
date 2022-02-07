using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramUpdater.ExceptionHandlers
{
    /// <summary>
    /// Base interface for <see cref="ExceptionHandler{T}"/>
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Handle only when the <see cref="Exception"/> occured in specified <see cref="IUpdateHandler"/>s
        /// </summary>
        public IEnumerable<Type> AllowedHandlers { get; }

        /// <summary>
        /// Your <see cref="Exception"/> type.
        /// </summary>
        public Type ExceptionType { get; }

        /// <summary>
        /// Handle only when <see cref="Exception.Message"/> matches a text.
        /// </summary>
        public Filter<string>? MessageMatch { get; }

        /// <summary>
        /// A callback function that will be called when the error catched.
        /// </summary>
        public Func<Exception, Task> Callback { get; }

        /// <summary>
        /// Checks if a message is matched.
        /// </summary>
        public bool MessageMatched(string message)
        {
            if (MessageMatch == null) return true;

            if (message == null) return false;

            return MessageMatch.TheyShellPass(message);
        }

        /// <summary>
        /// Checks if handler type is allowed for this exception handler.
        /// </summary>
        /// <param name="handlerType">Type of handler.</param>
        /// <returns></returns>
        public bool IsAllowedHandler(Type handlerType)
            => AllowedHandlers.Any(x => x == handlerType);
    }
}
