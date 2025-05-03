using TelegramUpdater.Helpers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extensions to access extra data specialized to a handling scope layer or group.
/// </summary>
public static class TempDataHoldersExtensions
{
    /// <summary>
    /// Get scope id for this handler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.ScopeId ScopeId<T>(this IContainer<T> container)
        where T: class
        => new(container.Input.ScopeId);

    /// <summary>
    /// Get layer id for this handler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.LayerId LayerId<T>(this IContainer<T> container)
        where T : class
        => new(container.Input.ScopeId, container.Input.LayerId);

    /// <summary>
    /// Get group id for this handler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.GroupId GroupId<T>(this IContainer<T> container)
        where T : class
        => new(container.Input.ScopeId, container.Input.LayerId, container.Input.Group);

    /// <summary>
    /// Get handler id for this handler.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.HandlerId HandlerId<T>(this IContainer<T> container)
        where T : class
        => new(container.Input.ScopeId, container.Input.LayerId, container.Input.Group, container.Input.Index);
}
