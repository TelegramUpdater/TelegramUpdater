#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateHandlers;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of extension methods for <see cref="Message"/> handlers
/// </summary>
/// <remarks>
/// This extension methods are only available using `this` keyword.
/// </remarks>
public static class MessageHandlerExtensions
{
    /// <summary>
    /// This is a test method and dose nothing.
    /// </summary>
    /// <param name="handler"></param>
    public static void BlahMessage(this IUpdateHandler<Message> handler)
    {
        // This is a placeholder for the actual implementation.
        // The method should be implemented according to the specific requirements.
    }
}
