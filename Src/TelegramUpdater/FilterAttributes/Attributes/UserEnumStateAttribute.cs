using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Use this to create your own user enum filter attribute class.
/// </summary>
/// <typeparam name="TEnum">The enum you like to use.</typeparam>
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
public abstract class UserEnumStateAttribute<TEnum> : FilterAttributeBuilder
    where TEnum : struct, Enum
{
    /// <param name="stateKeeperName">The name of state keeper that you registered.</param>
    /// <param name="state">The state.</param>
    ///  public UserEnumStateAttribute(string stateKeeperName, TEnum state) : base(
    protected UserEnumStateAttribute(string stateKeeperName, TEnum state) : base(builder => builder
       .AddFilterForUpdate(
           new MessageUserHasEnumStateFilter<TEnum>(stateKeeperName, state))
       .AddFilterForUpdate(
           new CallbackQueryUserHasEnumStateFilter<TEnum>(stateKeeperName, state)))
    {
    }

    /// <param name="state">The state.</param>
    ///  public UserEnumStateAttribute(string stateKeeperName, TEnum state) : base(
    protected UserEnumStateAttribute(TEnum state) : base(builder => builder
       .AddFilterForUpdate(
           new MessageUserHasEnumStateFilter<TEnum>(Extensions.DefaultEnumStateKeeperName<TEnum>(), state))
       .AddFilterForUpdate(
           new CallbackQueryUserHasEnumStateFilter<TEnum>(Extensions.DefaultEnumStateKeeperName<TEnum>(), state)))
    {
    }
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