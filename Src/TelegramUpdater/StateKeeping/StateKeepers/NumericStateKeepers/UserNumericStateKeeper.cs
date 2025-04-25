namespace TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

/// <summary>
/// A numeric state keeper that increases and decreases
/// an <see cref="int"/> state for a <see cref="User"/>.
/// </summary>
/// <param name="range">
/// If range is specified (not null), states less than range.Start or larger than range.End
/// will not be acceptable.
/// </param>
public sealed class UserNumericStateKeeper(Range? range = null)
    : AbstractNumericStateKeeper<User>
{
    /// <inheritdoc/>
    protected override Func<User, long> KeyResolver => (user) => user.Id;

    /// <inheritdoc/>
    public override Range? StateRange => range;
}
