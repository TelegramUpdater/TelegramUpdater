﻿using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// Base class for update containers. ( Used while handling updates )
/// </summary>
public interface IContainer: IBaseContainer
{
    /// <summary>
    /// Raw inputs of the handler.
    /// </summary>
    public HandlerInput Input { get; }

    /// <summary>
    /// Processing info for this update.
    /// </summary>
    public ShiningInfo<long, Update> ShiningInfo => Input.ShiningInfo;

    internal IChangeToken? ScopeChangeToken => Input.ScopeChangeToken;

    internal IChangeToken? LayerChangeToken => Input.LayerChangeToken;

    /// <summary>
    /// Container may contain extra data based on the filter applied on handler.
    /// You can find that data here.
    /// </summary>
    /// <remarks>
    /// Eg: a <see cref="ReadyFilters.OnCommand(string[])"/> may insert an string array with key <c>args</c>.
    /// </remarks>
    /// <param name="key">Data key, based on applied filter.</param>
    /// <returns>
    /// An object that you may need to cast.
    /// <para>Eg: var args = (string[])container["args"];</para>
    /// </returns>
    public object this[string key] { get; }

    /// <summary>
    /// Checks if a key is available to fetch from <see cref="this[string]"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><see langword="true"/> if the key is available.</returns>
    public bool ContainsKey(string key);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetExtraData<T>(string key, [NotNullWhen(true)] out T? value);
}

/// <summary>
/// A sub interface of <see cref="IContainer"/>, made for simplicity.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IContainer<T> : IContainer, IBaseContainer<T> where T : class
{

}
