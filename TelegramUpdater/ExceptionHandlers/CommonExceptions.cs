using Telegram.Bot.Exceptions;
using TelegramUpdater.Filters;

namespace TelegramUpdater.ExceptionHandlers
{
    public static class CommonExceptions
    {
        /// <summary>
        /// Handle parse exception occures in api requests (Bad Request: can't parse entities)
        /// </summary>
        public static ExceptionHandler<ApiRequestException> ParsingException(
            Func<IUpdater, Exception, Task> callback,
            Type[]? allowedHandlers = null)
        {
            return new ExceptionHandler<ApiRequestException>(
                callback,
                new StringRegex("^Bad Request: can't parse entities"),
                allowedHandlers, false);
        }
    }
}
