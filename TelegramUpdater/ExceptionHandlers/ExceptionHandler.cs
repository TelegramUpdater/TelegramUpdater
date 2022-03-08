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

        /// <inheritdoc/>
        public IEnumerable<Type>? AllowedHandlers { get; }

        /// <inheritdoc/>
        public Type ExceptionType { get; }

        /// <inheritdoc/>
        public Filter<string>? MessageMatch { get; }

        /// <inheritdoc/>
        public Func<IUpdater, Exception, Task> Callback { get; }

        /// <inheritdoc/>
        public bool Inherit { get; }
    }
}
