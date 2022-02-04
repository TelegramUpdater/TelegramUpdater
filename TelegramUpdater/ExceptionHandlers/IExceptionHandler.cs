namespace TelegramUpdater.ExceptionHandlers
{
    public interface IExceptionHandler
    {
        public Type ExceptionType { get; }

        public Filter<string>? MessageMatch { get; }

        public Func<Exception, Task> Callback { get; }


        public bool MessageMatched(string message)
        {
            if (MessageMatch == null) return true;

            if (message == null) return false;

            return MessageMatch.TheyShellPass(message);
        }
    }
}
