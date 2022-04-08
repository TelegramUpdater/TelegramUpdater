using TelegramUpdater.StateKeeping.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter users with an specified numeric filter.
/// </summary>
/// <remarks>
/// You should add an <see cref="StateKeeping.StateKeepers.NumericStateKeepers.UserNumericStateKeeper"/>
/// to the updater first.
/// </remarks>
public sealed class UserNumericState : FilterAttributeBuilder
{
    /// <summary>
    /// Initialize a new instance of <see cref="UserNumericState"/>.
    /// </summary>
    /// <param name="stateKeeperName">Name of state keeper.</param>
    /// <param name="state">The state that user should have.</param>
    /// <param name="anyState">If <paramref name="state"/> is not a care.</param>
    public UserNumericState(string stateKeeperName, int state = 0, bool anyState = false)
        : base(builder => builder.AddFilterForUpdate(
            new MessageUserHasNumericStateFilter(stateKeeperName, anyState? null: state))
        .AddFilterForUpdate(
            new CallbackQueryUserHasNumericStateFilter(stateKeeperName, anyState ? null : state)))
    {
    }
}
