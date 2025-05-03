using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Set an item that expires when handling scope for this handler ends.
    /// </summary>
    public static bool SetScopeItem<T, TKey, TValue>(
        this IContainer<T> container, TKey key, TValue value)
        where T : class
    {
        if (container.ScopeChangeToken is null) return false;

        container.Updater.SetScopeItem(
            container.ScopeChangeToken, container.ScopeId(), key, value);

        return true;
    }

    /// <summary>
    /// Remove an item attached with this handler's scope id.
    /// </summary>
    public static void RemoveScopeItem<T, TKey>(this IContainer<T> container, TKey key)
        where T : class
        => container.Updater.RemoveScopeItem(container.ScopeId(), key);

    /// <summary>
    /// Get an item that was set in handler's scope id.
    /// </summary>
    public static bool TryGetScopeItem<T, TKey>(this IContainer<T> container, TKey key, [NotNullWhen(true)] out object? value)
        where T : class
        => container.Updater.TryGetScopeItem(container.ScopeId(), key, out value);

    /// <summary>
    /// Set an item that expires when handling layer for this handler ends.
    /// </summary>
    public static bool SetLayerItem<T, TKey, TValue>(
        this IContainer<T> container, TKey key, TValue value)
        where T : class
    {
        if (container.ScopeChangeToken is null) return false;

        container.Updater.SetLayerItem(
            container.ScopeChangeToken, container.LayerId(), key, value);

        return true;
    }

    /// <summary>
    /// Remove an item attached with this handler's layer id.
    /// </summary>
    public static void RemoveLayerItem<T, TKey>(
        this IContainer<T> container, TKey key)
        where T : class
        => container.Updater.RemoveLayerItem(container.LayerId(), key);

    /// <summary>
    /// Get an item that was set in handler's layer id.
    /// </summary>
    public static bool TryGetLayerItem<T, TKey>(this IContainer<T> container, TKey key, [NotNullWhen(true)] out object? value)
        where T : class
        => container.Updater.TryGetLayerItem(container.LayerId(), key, out value);
}
