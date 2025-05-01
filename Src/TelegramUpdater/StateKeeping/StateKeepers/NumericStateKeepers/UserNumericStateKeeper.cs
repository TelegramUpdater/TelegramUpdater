using TelegramUpdater.StateKeeping.Storages;

namespace TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

/// <summary>
/// A numeric state keeper that increases and decreases
/// an <see cref="int"/> state for a <see cref="User"/>.
/// </summary>
/// <param name="storage"></param>
/// <param name="range">
/// If range is specified (not null), states less than range.Start or larger than range.End
/// will not be acceptable.
/// </param>
public class UserNumericStateKeeper<TStorage>(
    TStorage storage, Range? range = null)
    : AbstractNumericStateKeeper<long, User, TStorage>(storage)
    where TStorage : IStateKeeperStorage<long, int>
{
    /// <inheritdoc/>
    protected override Func<User, long> KeyResolver => (user) => user.Id;

    /// <inheritdoc/>
    public override Range? StateRange { get; } = range;
}

/// <inheritdoc/>
public sealed class MemoryUserNumericStateKeeper(Range? range = null)
    : UserNumericStateKeeper<MemoryCacheStorage<long, int>>(new(), range)
{
}
