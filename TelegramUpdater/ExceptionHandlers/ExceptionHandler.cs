using TelegramUpdater.UpdateHandlers;

namespace TelegramUpdater.ExceptionHandlers
{
    /// <summary>
    /// Can be used to handle <see cref="Exception"/> of specified type (<typeparamref name="TException"/>) in <see cref="Updater"/>.
    /// </summary>
    /// <typeparam name="TException">Your <see cref="Exception"/> type.</typeparam>
    public class ExceptionHandler<TException> : IExceptionHandler where TException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExceptionHandler{T}"/>
        /// where <typeparamref name="TException"/> is a <see cref="Exception"/>
        /// </summary>
        /// <param name="callback">
        /// A callback function that will be called when the error catches.
        /// </param>
        /// <param name="messageMatch">
        /// Handle only when <see cref="Exception.Message"/> matches a text.
        /// </param>
        /// <param name="allowedHandlers">
        /// Handle only when the <see cref="Exception"/> occurred in specified
        /// <see cref="IUpdateHandler"/>s
        /// <para>Leave null to handle all.</para>
        /// </param>
        /// <param name="inherit">
        /// Indicates if this handler should catch all of exceptions
        /// that are inherited from <typeparamref name="TException"/>.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// If any of <paramref name="allowedHandlers"/> are not.
        /// <see cref="IUpdateHandler"/></exception>
        public ExceptionHandler(
            Func<IUpdater, Exception, Task> callback,
            Filter<string>? messageMatch = default,
            Type[]? allowedHandlers = null,
            bool inherit = false)
        {
            MessageMatch = messageMatch;
            ExceptionType = typeof(TException);
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

        /// <summary>
        /// Allowed handlers to handle exceptions occurred inside them.
        /// </summary>
        /// <value></value>
        public IEnumerable<Type>? AllowedHandlers { get; }

        /// <summary>
        /// Type of exception to catch.
        /// </summary>
        /// <value></value>
        public Type ExceptionType { get; }

        /// <summary>
        /// A filter to match <see cref="Exception.Message"/>.
        /// </summary>
        /// <value></value>
        public Filter<string>? MessageMatch { get; }

        /// <summary>
        /// A function that will be called when the error catches.
        /// </summary>
        /// <value></value>
        public Func<IUpdater, Exception, Task> Callback { get; }

        /// <summary>
        /// Indicates if this handler should catch all of exceptions.
        /// </summary>
        /// <value></value>
        public bool Inherit { get; }
    }
}
