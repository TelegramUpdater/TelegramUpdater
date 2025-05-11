namespace TelegramUpdater.UpdateHandlers.Controller.Attributes;

/// <summary>
/// Mark a parameter to be resolved from the service provider.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public class ResolveServiceAttribute : Attribute
{
    /// <summary>
    /// If the service is required or not.
    /// </summary>
    public bool Required { get; set; } = true;
}
