namespace TelegramUpdater.UpdateHandlers.Controller.Attributes;

/// <summary>
/// Mark a method as an action handler.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class HandlerActionAttribute : Attribute
{
    /// <summary>
    /// Determines if this is an endpoint action. if got handled.
    /// </summary>
    public bool Endpoint { get; set; } = true;

    /// <summary>
    /// Handling priority.
    /// </summary>
    public int Group { get; set; } = 0;

    /// <summary>
    /// Determines if the action should be ignored if the parameters are not found.
    /// Otherwise, an exception will be thrown.
    /// </summary>
    public bool IgnoreIfParametersNotFound { get; set; } = true;
}
