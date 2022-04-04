using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater.ExceptionHandlers
{
    /// <summary>
    /// Base interface for <see cref="ExceptionHandler{T}"/>
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Handle only when the <see cref="Exception"/>
        /// occurred in specified <see cref="IUpdateHandler"/>s.
        /// </summary>
        /// <remarks>If it's null, mean all!</remarks>
        public IEnumerable<Type>? AllowedHandlers { get; }

        /// <summary>
        /// Your <see cref="Exception"/> type.
        /// </summary>
        public Type ExceptionType { get; }

        /// <summary>
        /// Handle only when <see cref="Exception.Message"/> matches a text.
        /// </summary>
        public Filter<string>? MessageMatch { get; }

        /// <summary>
        /// A callback function that will be called when the error catches.
        /// </summary>
        public Func<IUpdater, Exception, Task> Callback { get; }

        /// <summary>
        /// Allow inherited exceptions and not only exact type.
        /// </summary>
        public bool Inherit { get; }

        internal bool TypeIsMatched(Type typeOfException)
        {
            if (!Inherit)
            {
                return ExceptionType == typeOfException;
            }
            else
            {
                return ExceptionType.IsAssignableFrom(typeOfException);
            }
        }

        /// <summary>
        /// Checks if a message is matched.
        /// </summary>
        internal bool MessageMatched(IUpdater updater, string message)
        {
            if (MessageMatch == null) return true;

            if (message == null) return false;

            return MessageMatch.TheyShellPass(updater, message);
        }

        /// <summary>
        /// Checks if handler type is allowed for this exception handler.
        /// </summary>
        /// <param name="handlerType">Type of handler.</param>
        /// <returns></returns>
        internal bool IsAllowedHandler(Type handlerType)
        {
            if (handlerType == null)
                throw new ArgumentNullException(nameof(handlerType));

            if (AllowedHandlers == null) return true;

            return AllowedHandlers.Any(x => x == handlerType);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExceptionHandler{T}"/>
        /// where <typeparamref name="TException"/> is a <see cref="Exception"/>
        /// </summary>
        /// <typeparam name="TException">
        /// Your <see cref="Exception"/> type.
        /// </typeparam>
        /// <typeparam name="THandler">
        /// Your <see cref="IUpdateHandler"/>
        /// </typeparam>
        /// <param name="callback">
        /// A callback function that will be called when the error catches.
        /// </param>
        /// <param name="messageMatch">
        /// Handle only when <see cref="Exception.Message"/> matches a text.
        /// </param>
        /// <returns></returns>
        public static ExceptionHandler<TException> ExceptionsInHandler<TException, THandler>(
            Func<IUpdater, Exception, Task> callback,
            Filter<string>? messageMatch = default)
            where TException : Exception where THandler : IUpdateHandler
                => new ExceptionHandler<TException>(
                    callback, messageMatch, new[] { typeof(THandler) });
    }
}
