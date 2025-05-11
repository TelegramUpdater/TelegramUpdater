namespace TelegramUpdater.UpdateHandlers.Controller.Attributes;

/// <summary>
/// Mark a method as an action handler.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class HandlerActionAttribute : Attribute
{
    /// <summary>
    /// Handling priority.
    /// </summary>
    public int Group { get; set; }
}
