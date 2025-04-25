using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Use this to create your own user enum filter attribute class.
/// </summary>
/// <typeparam name="TEnum">The enum you like to use.</typeparam>
/// <param name="stateKeeperName">The name of state keeper that you registered.</param>
/// <param name="state">The state.</param>
/// <example>
/// <code>
/// enum IsFreaked
/// {
///     NotFreaked,
///     Freaked,
/// }
///
/// sealed class IsFreackedAttribute(IsFreaked freacked)
///     : UserEnumStateAttribute&lt;IsFreaked&gt;("myFreakingStateKeeper", freacked);
///
/// [IsFreacked(IsFreaked.Freaked)]
/// sealed class FreakingHandler { }
/// </code>
/// </example>
public abstract class UserEnumStateAttribute<TEnum>(string stateKeeperName, TEnum state)
    : FilterAttributeBuilder(
        builder => builder
            .AddFilterForUpdate(
                new MessageUserHasEnumStateFilter<TEnum>(stateKeeperName, state))
            .AddFilterForUpdate(
                new CallbackQueryUserHasEnumStateFilter<TEnum>(stateKeeperName, state)))
    where TEnum : struct, Enum
{
}

// Example
enum IsFreaked
{
    NotFreaked,
    Freaked,
}

sealed class IsFreackedAttribute(IsFreaked freacked)
    : UserEnumStateAttribute<IsFreaked>("myFreakingStateKeeper", freacked);

[IsFreacked(IsFreaked.Freaked)]
sealed class FreakingHandler { }