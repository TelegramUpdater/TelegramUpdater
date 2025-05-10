using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Helpers;

namespace TelegramUpdater.Filters;

/// <summary>
/// Determines a region where a data can be fetched from <see cref="IUpdater"/>.
/// </summary>
public enum DataRegion
{
    /// <summary>
    /// Any data on <see cref="IUpdater"/>.
    /// </summary>
    Any,

    /// <summary>
    /// Data only inside the handlers handling scope.
    /// </summary>
    Scope,

    /// <summary>
    /// Data only inside the handlers handling layer.
    /// </summary>
    Layer,
}

/// <summary>
/// Acquire a value from <see cref="IUpdater"/>'s data and do some checks on it.
/// </summary>
public class UpdaterDataFilter<T> : UpdaterFilter<T>
{
    /// <summary>
    /// Resolve a key for targeted data.
    /// </summary>
    protected Func<UpdaterFilterInputs<T>, string?> DataKeyResolver { get; }

    /// <summary>
    /// Do some extra checks on acquired data.
    /// </summary>
    protected Func<object, bool>? DataCheck { get; }

    /// <summary>
    /// Determines if the data should be removed after finding it.
    /// </summary>
    public bool ThenRemove { get; init; } = false;

    /// <summary>
    /// Determines if the data should be removed if <see cref="ThenRemove"/> and the result of <see cref="DataCheck"/> is true.
    /// </summary>
    public bool RemoveOnSuccessfulCheck { get; init; } = true;

    /// <inheritdoc cref="DataRegion"/>
    public DataRegion Region { get; init; } = DataRegion.Any;

    /// <summary>
    /// Create a new instance of <see cref="UpdaterDataFilter{T}"/>.
    /// </summary>
    /// <param name="dataKeyResolver">Resolve a key for targeted data.</param>
    /// <param name="dataCheck">Do some extra checks on acquired data.</param>
    public UpdaterDataFilter(
        Func<UpdaterFilterInputs<T>, string?> dataKeyResolver,
        Func<object, bool>? dataCheck = default)
    {
        DataKeyResolver = dataKeyResolver
            ?? throw new ArgumentNullException(nameof(dataKeyResolver));
        DataCheck = dataCheck;
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterDataFilter{T}"/>.
    /// </summary>
    /// <param name="dataKeyResolver">Resolve a key for targeted data.</param>
    /// <param name="dataCheck">Do some extra checks on acquired data.</param>
    public UpdaterDataFilter(
        Func<T, string?> dataKeyResolver,
        Func<object, bool>? dataCheck = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(dataKeyResolver);
#else
        if (dataKeyResolver is null)
        {
            throw new ArgumentNullException(nameof(dataKeyResolver));
        }
#endif

        DataKeyResolver = (inputs) => dataKeyResolver(inputs.Input);
        DataCheck = dataCheck;
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterDataFilter{T}"/>.
    /// </summary>
    /// <param name="key">A key for targeted data.</param>
    /// <param name="dataCheck">Do some extra checks on acquired data.</param>
    public UpdaterDataFilter(
        string key,
        Func<object, bool>? dataCheck = default)
    {
        DataKeyResolver = (_) => key;
        DataCheck = dataCheck;
    }

    /// <summary>
    /// A function that acquires a value from updater.
    /// </summary>
    /// <remarks>
    /// It's resolved using <see cref="Region"/> by default.
    /// </remarks>
    protected virtual bool TryGetValue(
        UpdaterFilterInputs<T> input, string key, [NotNullWhen(true)] out object? value)
        => Region switch
        {
            DataRegion.Any => input.Updater.TryGetValue(key, out value),
            DataRegion.Scope => input.Updater.TryGetScopeItem(new HandlingStoragesKeys.ScopeId(input.ScopeId), key, out value),
            DataRegion.Layer => input.Updater.TryGetLayerItem(new HandlingStoragesKeys.LayerId(input.ScopeId, input.LayerInfo.Key), key, out value),
            _ => throw new InvalidOperationException("Invalid region."),
        };

    /// <summary>
    /// A function that removes a key from updater.
    /// </summary>
    /// <remarks>
    /// It's resolved using <see cref="Region"/> by default.
    /// </remarks>
    protected virtual void Remove(UpdaterFilterInputs<T> input, string key)
    {
        switch (Region)
        {
            case DataRegion.Any:
                input.Updater.RemoveItem(key);
                break;
            case DataRegion.Scope:
                input.Updater.RemoveScopeItem(new HandlingStoragesKeys.ScopeId(input.ScopeId), key);
                break;
            case DataRegion.Layer:
                input.Updater.RemoveLayerItem(new HandlingStoragesKeys.LayerId(input.ScopeId, input.LayerInfo.Key), key);
                break;
        }
    }

    /// <inheritdoc/>
    public override bool TheyShellPass(UpdaterFilterInputs<T> input)
    {
        var key = DataKeyResolver(input);

        if (key == null) return false;

        if (TryGetValue(input, key, out var value))
        {
            var result = DataCheck is null || DataCheck(value);

            if (ThenRemove)
            {
                if (RemoveOnSuccessfulCheck)
                {
                    if (result)
                    {
                        input.Updater.RemoveItem(key);
                    }
                }
                else
                {
                    input.Updater.RemoveItem(key);
                }
            }

            return result;
        }

        return false;
    }
}
