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
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.ScopeId ScopeId(this IContainer container)
        => new(container.Input.ScopeId);

    /// <summary>
    /// Get layer id for this handler.
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.LayerId LayerId(this IContainer container)
        => new(container.Input.ScopeId, container.Input.LayerInfo.Key);

    /// <summary>
    /// Get group id for this handler.
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.GroupId GroupId(this IContainer container)
        => new(container.Input.ScopeId, container.Input.LayerInfo.Key, container.Input.Group);

    /// <summary>
    /// Get handler id for this handler.
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static HandlingStoragesKeys.HandlerId HandlerId(this IContainer container)
        => new(container.Input.ScopeId, container.Input.LayerInfo.Key, container.Input.Group, container.Input.Index);

    /// <summary>
    /// Set an item that expires when handling scope for this handler ends.
    /// </summary>
    public static bool SetScopeItem<TValue>(
        this IContainer container, string key, TValue value)
    {
        if (container.ScopeChangeToken is null) return false;

        container.Updater.SetScopeItem(
            container.ScopeChangeToken, container.ScopeId(), key, value);

        return true;
    }

    /// <summary>
    /// Remove an item attached with this handler's scope id.
    /// </summary>
    public static void RemoveScopeItem(this IContainer container, string key)
        => container.Updater.RemoveScopeItem(container.ScopeId(), key);

    /// <summary>
    /// Get an item that was set in handler's scope id.
    /// </summary>
    public static bool TryGetScopeItem<TValue>(this IContainer container, string key, [NotNullWhen(true)] out TValue? value)
        => container.Updater.TryGetScopeItem(container.ScopeId(), key, out value);

    /// <summary>
    /// Set an item that expires when handling layer for this handler ends.
    /// </summary>
    public static bool SetLayerItem<TValue>(
        this IContainer container, string key, TValue value)
    {
        if (container.ScopeChangeToken is null) return false;

        container.Updater.SetLayerItem(
            container.ScopeChangeToken, container.LayerId(), key, value);

        return true;
    }

    /// <summary>
    /// Remove an item attached with this handler's layer id.
    /// </summary>
    public static void RemoveLayerItem(
        this IContainer container, string key)
        => container.Updater.RemoveLayerItem(container.LayerId(), key);

    /// <summary>
    /// Get an item that was set in handler's layer id.
    /// </summary>
    public static bool TryGetLayerItem<TValue>(this IContainer container, string key, [NotNullWhen(true)] out TValue? value)
        => container.Updater.TryGetLayerItem(container.LayerId(), key, out value);

    #region Composite key
    /// <summary>
    /// Set a composite key item that expires when handling scope for this handler ends.
    /// </summary>
    public static bool SetCompositeScopeItem<TValue>(
        this IContainer container, string firstKey, string secondKey, TValue value)
    {
        if (container.ScopeChangeToken is null) return false;

        container.Updater.SetCompositeScopeItem(
            container.ScopeChangeToken, container.ScopeId(), firstKey, secondKey, value);

        return true;
    }

    /// <summary>
    /// Remove a composite key item attached with this handler's scope id.
    /// </summary>
    public static void RemoveCompositeScopeItem(
        this IContainer container, string firstKey, string secondKey)
        => container.Updater.RemoveCompositeScopeItem(container.ScopeId(), firstKey, secondKey);

    /// <summary>
    /// Get a composite key item that was set in handler's scope id.
    /// </summary>
    public static bool TryGetCompositeScopeItem<TValue>(
        this IContainer container, string firstKey, string secondKey, [NotNullWhen(true)] out TValue? value)
        => container.Updater.TryGetCompositeScopeItem(container.ScopeId(), firstKey, secondKey, out value);

    /// <summary>
    /// Set a composite key item that expires when handling layer for this handler ends.
    /// </summary>
    public static bool SetCompositeLayerItem<TValue>(
        this IContainer container, string firstKey, string secondKey, TValue value)
    {
        if (container.LayerChangeToken is null) return false;

        container.Updater.SetCompositeLayerItem(
            container.LayerChangeToken, container.LayerId(), firstKey, secondKey, value);

        return true;
    }

    /// <summary>
    /// Remove a composite key item attached with this handler's layer id.
    /// </summary>
    public static void RemoveCompositeLayerItem(
        this IContainer container, string firstKey, string secondKey)
        => container.Updater.RemoveCompositeLayerItem(container.LayerId(), firstKey, secondKey);

    /// <summary>
    /// Get a composite key item that was set in handler's layer id.
    /// </summary>
    public static bool TryGetCompositeLayerItem<TValue>(
        this IContainer container, string firstKey, string secondKey, [NotNullWhen(true)] out TValue? value)
        => container.Updater.TryGetCompositeLayerItem(container.LayerId(), firstKey, secondKey, out value);
    #endregion

    #region User specified

    #endregion
}
