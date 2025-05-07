using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using TelegramUpdater.UpdateContainer.Tags;
using static TelegramUpdater.UpdaterExtensions;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// A set of extension methods for containers that are tagged with something like
/// <see cref="ISenderIdExtractable"/>.
/// </summary>
public static class TaggedExtensions
{
    /// <summary>
    /// Set a user item.
    /// </summary>
    public static bool SetUserItem<C, TValue>(
        this C container, string key, TValue value, MemoryCacheEntryOptions? options = default)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        var senderId = container.GetSenderId();

        if (senderId == null) return false;

        container.Updater.SetCompositeItem(
            senderId.Value.ToString(CultureInfo.InvariantCulture),
            key,
            value,
            options);

        return true;
    }

    /// <summary>
    /// Set a user item.
    /// </summary>
    public static void RemoveUserItem<C>(this C container, string key)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        var senderId = container.GetSenderId();

        if (senderId == null) return;

        container.Updater.RemoveCompositeItem(
            senderId.Value.ToString(CultureInfo.InvariantCulture), key);
    }

    /// <summary>
    /// Set a user item.
    /// </summary>
    public static bool TryGetUserItem<C, TValue>(
        this C container,
        string key,
        [NotNullWhen(true)] out TValue? value)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        var senderId = container.GetSenderId();

        if (senderId == null)
        {
            value = default;
            return false;
        }

        return container.Updater.TryGetCompositeItem(
            senderId.Value.ToString(CultureInfo.InvariantCulture), key, out value);
    }

    /// <summary>
    /// Set a user key item that expires when handling scope for this handler ends.
    /// </summary>
    public static bool SetUserScopeItem<C, TValue>(
        this C container, string key, TValue value)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        var senderId = container.GetSenderId();

        if (senderId == null) return false;

        return container.SetCompositeScopeItem(
            senderId.Value.ToString(CultureInfo.InvariantCulture),
            key,
            value);
    }

    /// <summary>
    /// Remove a user key item attached with this handler's scope id.
    /// </summary>
    public static void RemoveUserScopeItem<C>(
        this C container, string key)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        if (container.GetSenderId() is long senderId)
            container.Updater.RemoveCompositeScopeItem(container.ScopeId(), senderId.ToString(CultureInfo.InvariantCulture), key);
    }

    /// <summary>
    /// Get a user key item that was set in handler's scope id.
    /// </summary>
    public static bool TryGetUserScopeItem<C, TValue>(
        this C container, string key, [NotNullWhen(true)] out TValue? value)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        if (container.GetSenderId() is long senderId)
            return container.Updater.TryGetCompositeScopeItem(container.ScopeId(), senderId.ToString(CultureInfo.InvariantCulture), key, out value);

        value = default;
        return false;
    }

    /// <summary>
    /// Set a user key item that expires when handling layer for this handler ends.
    /// </summary>
    public static bool SetUserLayerItem<C, TValue>(
        this C container, string key, TValue value)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        var senderId = container.GetSenderId();

        if (senderId == null) return false;

        return container.SetCompositeLayerItem(
            senderId.Value.ToString(CultureInfo.InvariantCulture),
            key,
            value);
    }

    /// <summary>
    /// Remove a user key item attached with this handler's layer id.
    /// </summary>
    public static void RemoveUserLayerItem<C>(
        this C container, string key)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        if (container.GetSenderId() is long senderId)
            container.Updater.RemoveCompositeLayerItem(container.LayerId(), senderId.ToString(CultureInfo.InvariantCulture), key);
    }

    /// <summary>
    /// Get a user key item that was set in handler's layer id.
    /// </summary>
    public static bool TryGetUserLayerItem<C>(
        this C container, string key, [NotNullWhen(true)] out object? value)
        where C : IUpdateContainer, ISenderIdExtractable
    {
        if (container.GetSenderId() is long senderId)
            return container.Updater.TryGetCompositeLayerItem(container.LayerId(), senderId.ToString(CultureInfo.InvariantCulture), key, out value);

        value = null;
        return false;
    }
}
