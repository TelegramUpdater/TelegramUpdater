namespace TelegramUpdater.StateKeeping.StateKeepers.NumericStateKeepers;

/// <summary>
/// A numeric state keeper that increases and decreases
/// an <see cref="int"/> state for a <see cref="User"/>.
/// </summary>
public sealed class UserNumericStateKeeper : AbstractNumericStateKeeper<User>
{
    /// <inheritdoc/>
    protected override Func<User, long> KeyResolver => (user) => user.Id;
}
