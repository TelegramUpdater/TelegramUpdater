#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handler propagation extensions.
/// </summary>
public static class PropagationExtensions
{
    /// <summary>
    /// Returns the value if not null, or throws <see cref="ContinuePropagationException"/>
    /// to stop handling this handler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="anything"></param>
    /// <returns></returns>
    /// <exception cref="ContinuePropagationException"></exception>
    public static T ContinueIfNull<T>(this T? anything)
    {
        return anything ?? throw new ContinuePropagationException();
    }

    /// <summary>
    /// Returns the value if not null, or throws <see cref="ContinuePropagationException"/>
    /// to stop handling this handler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="anything"></param>
    /// <returns></returns>
    /// <exception cref="ContinuePropagationException"></exception>
    public static T ContinueIfNull<T>(this Nullable<T> anything)
        where T: struct
    {
        return anything ?? throw new ContinuePropagationException();
    }
}
