using System;

namespace TelegramUpdater
{
    /// <summary>
    /// All pending handlers for this update will be ignored after throwing this.
    /// </summary>
    public class StopPropagation : Exception
    {
        /// <summary>
        /// All pending handlers for this update will be ignored after throwing this.
        /// </summary>
        public StopPropagation()
        {
        }
    }

    /// <summary>
    /// Continue to the next pending handler for this update and ignore the rest of this handler.
    /// </summary>
    public class ContinuePropagation : Exception
    {
        /// <summary>
        /// Continue to the next pending handler for this update and ignore the rest of this handler.
        /// </summary>
        public ContinuePropagation()
        {
        }
    }
}
