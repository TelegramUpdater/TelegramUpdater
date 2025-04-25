namespace TelegramUpdater;

/// <summary>
/// All pending handlers for this update will be ignored after throwing this.
/// </summary>
public class StopPropagationException : Exception
{
    /// <summary>
    /// All pending handlers for this update will be ignored after throwing this.
    /// </summary>
    public StopPropagationException()
    {
    }
}

/// <summary>
/// Continue to the next pending handler for this update and ignore the rest of this handler.
/// </summary>
public class ContinuePropagationException : Exception
{
    /// <summary>
    /// Continue to the next pending handler for this update and ignore the rest of this handler.
    /// </summary>
    public ContinuePropagationException()
    {
    }
}
