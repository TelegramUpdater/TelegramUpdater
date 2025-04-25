using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Filter users with an specified numeric filter.
/// </summary>
/// <remarks>
/// You should add an <see cref="StateKeeping.StateKeepers.NumericStateKeepers.UserNumericStateKeeper"/>
/// to the updater first.
/// </remarks>
/// <remarks>
/// Initialize a new instance of <see cref="UserNumericStateAttribute"/>.
/// </remarks>
/// <param name="stateKeeperName">Name of state keeper.</param>
/// <param name="state">The state that user should have.</param>
public sealed class UserNumericStateAttribute(string stateKeeperName, int state = 0)
    : FilterAttributeBuilder(
        builder => builder
            .AddFilterForUpdate(
                new MessageUserHasNumericStateFilter(stateKeeperName, state))
            .AddFilterForUpdate(
                new CallbackQueryUserHasNumericStateFilter(stateKeeperName, state)))
{
}
