using TelegramUpdater.StateKeeping.Storages;

namespace TelegramUpdater.StateKeeping.StateKeepers.EnumStateKeepers;

/// <inheritdoc/>
public class UserEnumStateKeeper<TEnum, TStorage>(TStorage storage)
    : AbstractEnumStateKeeper<long, TEnum, User, TStorage>(storage)
    where TEnum : struct, Enum
    where TStorage : IStateKeeperStorage<long, TEnum>
{
    /// <inheritdoc/>
    protected override Func<User, long> KeyResolver => (user) => user.Id;
}

/// <inheritdoc/>
public sealed class MemoryUserEnumStateKeeper<TEnum>()
    : UserEnumStateKeeper<TEnum, MemoryCacheStorage<long, TEnum>>(new())
    where TEnum : struct, Enum
{ }
