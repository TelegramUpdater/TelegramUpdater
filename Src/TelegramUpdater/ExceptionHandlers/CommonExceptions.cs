using Telegram.Bot.Exceptions;
using TelegramUpdater.Filters;

namespace TelegramUpdater.ExceptionHandlers
{
    /// <summary>
    /// A set of some common exception handlers.
    /// </summary>
    public static class CommonExceptions
    {
        /// <summary>
        /// Handle parse exception occurs in API requests
        /// (Bad Request: can't parse entities)
        /// </summary>
        public static ExceptionHandler<ApiRequestException> ParsingException(
            Func<IUpdater, Exception, Task> callback,
            Type[]? allowedHandlers = null)
        {
            return new ExceptionHandler<ApiRequestException>(
                callback,
                new StringRegex("^Bad Request: can't parse entities"),
                allowedHandlers, inherit: false);
        }
    }
}
