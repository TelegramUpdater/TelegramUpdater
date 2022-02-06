using System;
using System.Threading.Tasks;

namespace TelegramUpdater.ExceptionHandlers
{
    public class ExceptionHandler<T> : IExceptionHandler where T : Exception
    {
        public ExceptionHandler(
            Func<Exception, Task>  callback,
            Filter<string>? messageMatch = default)
        {
            MessageMatch = messageMatch;
            ExceptionType = typeof(T);
            Callback = callback;
        }

        public Type ExceptionType { get; }

        public Filter<string>? MessageMatch { get; }

        public Func<Exception, Task> Callback { get; }
    }
}
