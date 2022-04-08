namespace TelegramUpdater.StateKeeping.Filters;

/// <summary>
/// An abstract class to create filter on <see cref="StateKeepers.NumericStateKeepers.UserNumericStateKeeper"/>.
/// </summary>
/// <typeparam name="T">Type of update.</typeparam>
public abstract class UserHasNumericStateFilter<T> : Filter<T> where T : class
{
    /// <summary>
    /// Initialize a new instance of <see cref="UserHasNumericStateFilter{T}"/>.
    /// </summary>
    /// <param name="userResolver">
    /// A function to extract <see cref="User"/> from <typeparamref name="T"/>
    /// </param>
    /// <param name="stateKeeperName">Name of state keeper.</param>
    /// <param name="state">The state that user should have.</param>
    public UserHasNumericStateFilter(
        Func<T, User?> userResolver,
        string stateKeeperName,
        int? state = default) : base((updater, update) =>
        {
            var keeper = updater.GetUserNumericStateKeeper(stateKeeperName);

            var user = userResolver(update);

            if (user is null) return false;

            if (keeper.TryGetState(user, out var userState))
            {
                if (state is null) return true;

                return userState == state;
            }

            return false;
        })
    { }
}

/// <summary>
/// A <see cref="UserHasNumericStateFilter{T}"/> for <see cref="Message"/>s.
/// </summary>
public sealed class MessageUserHasNumericStateFilter
    : UserHasNumericStateFilter<Message>
{
    /// <summary>
    /// Initialize a new instance of <see cref="MessageUserHasNumericStateFilter"/>.
    /// </summary>
    /// <param name="stateKeeperName">Name of state keeper.</param>
    /// <param name="state">The state that user should have.</param>
    public MessageUserHasNumericStateFilter(string stateKeeperName, int? state = default)
        : base((message) => message.From, stateKeeperName, state)
    {
    }
}

/// <summary>
/// A <see cref="UserHasNumericStateFilter{T}"/> for <see cref="CallbackQuery"/>s.
/// </summary>
public sealed class CallbackQueryUserHasNumericStateFilter
    : UserHasNumericStateFilter<CallbackQuery>
{
    /// <summary>
    /// Initialize a new instance of <see cref="CallbackQueryUserHasNumericStateFilter"/>.
    /// </summary>
    /// <param name="stateKeeperName">Name of state keeper.</param>
    /// <param name="state">The state that user should have.</param>
    public CallbackQueryUserHasNumericStateFilter(string stateKeeperName, int? state = default)
        : base((callbackQuery) => callbackQuery.From, stateKeeperName, state)
    {
    }
}
