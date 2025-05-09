using System.Globalization;
using TelegramUpdater.Filters;
using static TelegramUpdater.UpdaterExtensions;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Attribute for <see cref="UpdaterDataFilter{T}"/>. 
/// </summary>
/// <remarks>
/// Should work on any type of handler.
/// </remarks>
public class UpdaterDataAttribute<T> : AbstractFilterAttribute
{
    private readonly UpdaterDataFilter<T> _filter;

    /// <summary>
    /// Determines if the data should be removed after finding it.
    /// </summary>
    public bool ThenRemove { get; init; } = false;

    /// <summary>
    /// Determines if the data should be removed if <see cref="ThenRemove"/> and the result of <see cref="UpdaterDataFilter{T}.DataCheck"/> is true.
    /// </summary>
    public bool RemoveOnSuccessfulCheck { get; init; } = true;

    /// <inheritdoc cref="DataRegion"/>
    public DataRegion Region { get; init; } = DataRegion.Any;

    /// <summary>
    /// Create a new instance of <see cref="UpdaterDataAttribute{T}"/>.
    /// </summary>
    /// <param name="dataKeyResolver">Resolve a key for targeted data.</param>
    /// <param name="dataCheck">Do some extra checks on acquired data.</param>
    public UpdaterDataAttribute(
        Func<UpdaterFilterInputs<T>, string?> dataKeyResolver,
        Func<object, bool>? dataCheck = default)
    {
        _filter = new(dataKeyResolver, dataCheck)
        {
            ThenRemove = ThenRemove,
            RemoveOnSuccessfulCheck = RemoveOnSuccessfulCheck,
            Region = Region,
        };
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterDataAttribute{T}"/>.
    /// </summary>
    /// <param name="dataKeyResolver">Resolve a key for targeted data.</param>
    /// <param name="dataCheck">Do some extra checks on acquired data.</param>
    public UpdaterDataAttribute(
        Func<T, string?> dataKeyResolver,
        Func<object, bool>? dataCheck = default)
    {
        _filter = new(dataKeyResolver, dataCheck)
        {
            ThenRemove = this.ThenRemove,
            RemoveOnSuccessfulCheck = this.RemoveOnSuccessfulCheck,
            Region = Region,
        };
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterDataAttribute{T}"/>.
    /// </summary>
    /// <param name="key">A key for targeted data.</param>
    /// <param name="dataCheck">Do some extra checks on acquired data.</param>
    public UpdaterDataAttribute(
        string key,
        Func<object, bool>? dataCheck = default)
    {
        _filter = new(key, dataCheck)
        {
            ThenRemove = this.ThenRemove,
            RemoveOnSuccessfulCheck = this.RemoveOnSuccessfulCheck,
            Region = Region,
        };
    }

    /// <inheritdoc/>
    protected internal override object GetFilterTypeOf(Type requestedType)
    {
        return _filter;
    }
}

/// <summary>
/// Determinate if a data with <see cref="CompositeKey"/> exists in updater.
/// </summary>
/// <remarks>
/// <see cref="CompositeKey.First"/> should be <see cref="User.Id"/>.
/// </remarks>
/// <param name="key">The other key.</param>
public class UserUpdaterDataExistsAttribute(string key) : AbstractUpdaterFilterAttribute
{
    /// <summary>
    /// Determines if the data should be removed after finding it.
    /// </summary>
    public bool ThenRemove { get; init; } = false;

    /// <summary>
    /// Determines if the data should be removed if <see cref="ThenRemove"/> and the result of <see cref="UpdaterDataFilter{T}.DataCheck"/> is true.
    /// </summary>
    public bool RemoveOnSuccessfulCheck { get; init; } = true;

    /// <inheritdoc cref="DataRegion"/>
    public DataRegion Region { get; init; } = DataRegion.Any;

    /// <inheritdoc/>
    protected internal override object GetUpdaterFilterTypeOf(Type requestedType)
    {
        if (requestedType == typeof(Message))
        {
            return new UpdaterDataFilter<Message>((message) =>
            {
                if (message.From is User user)
                    return new CompositeKey(user.Id.ToString(CultureInfo.InvariantCulture), key);
                return null;
            })
            {
                ThenRemove = this.ThenRemove,
                RemoveOnSuccessfulCheck = this.RemoveOnSuccessfulCheck,
                Region = Region,
            };
        }
        else if (requestedType == typeof(Message))
        {
            return new UpdaterDataFilter<CallbackQuery>((callbackQuery) =>
            {
                if (callbackQuery.From is User user)
                    return new CompositeKey(user.Id.ToString(CultureInfo.InvariantCulture), key);
                return null;
            })
            {
                ThenRemove = this.ThenRemove,
                RemoveOnSuccessfulCheck = this.RemoveOnSuccessfulCheck,
                Region = Region,
            };
        }

        throw new NotSupportedException(
            $"UserUpdaterDataExistsAttribute don't support input filter of {requestedType.Name}.");
    }
}