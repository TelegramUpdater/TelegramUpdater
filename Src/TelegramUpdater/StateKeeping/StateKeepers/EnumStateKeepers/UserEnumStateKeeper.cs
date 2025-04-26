namespace TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;

/// <inheritdoc/>
public class UserEnumStateKeeper<TEnum>
    : AbstractEnumStateKeeper<long, TEnum, User>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    protected override Func<User, long> KeyResolver => (user) => user.Id;
}
