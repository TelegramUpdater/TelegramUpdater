using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater
{
    public static class PropagationExtensions
    {
        /// <summary>
        /// All pending handlers for this update will be ignored after throwing this.
        /// </summary>
        public static void StopPropagation<T>(this UpdateContainerAbs<T> _)
            where T : class => throw new StopPropagation();

        /// <summary>
        /// Continue to the next pending handler for this update and ignore the rest of this handler.
        /// </summary>
        public static void ContinuePropagation<T>(this UpdateContainerAbs<T> _)
            where T : class => throw new ContinuePropagation();
    }
}
