namespace TelegramUpdater.Filters;

using StateKeeping.StateKeepers.NumericStateKeepers;

/// <summary>
/// An abstract class to create filter on <see cref="UserNumericStateKeeper"/>.
/// </summary>
/// <typeparam name="T">Type of update.</typeparam>
/// <remarks>
/// Initialize a new instance of <see cref="UserHasNumericStateFilter{T}"/>.
/// </remarks>
/// <param name="userResolver">
/// A function to extract <see cref="User"/> from <typeparamref name="T"/>
/// </param>
/// <param name="stateKeeperName">Name of state keeper.</param>
/// <param name="state">The state that user should have.</param>
public abstract class UserHasNumericStateFilter<T>(
    Func<T, User?> userResolver,
    string stateKeeperName,
    int state = default) : UpdaterFilter<T>((updater, update) =>
    {
        var keeper = updater.GetUserNumericStateKeeper(stateKeeperName);

        var user = userResolver(update);

        if (user is null) return false;

        return keeper.HasState(user, state);
    }) where T : class
{
}

/// <summary>
/// A <see cref="UserHasNumericStateFilter{T}"/> for <see cref="Message"/>s.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="MessageUserHasNumericStateFilter"/>.
/// </remarks>
/// <param name="stateKeeperName">Name of state keeper.</param>
/// <param name="state">The state that user should have.</param>
public sealed class MessageUserHasNumericStateFilter(string stateKeeperName, int state = default)
    : UserHasNumericStateFilter<Message>((message) => message.From, stateKeeperName, state)
{
}

/// <summary>
/// A <see cref="UserHasNumericStateFilter{T}"/> for <see cref="CallbackQuery"/>s.
/// </summary>
/// <remarks>
/// Initialize a new instance of <see cref="CallbackQueryUserHasNumericStateFilter"/>.
/// </remarks>
/// <param name="stateKeeperName">Name of state keeper.</param>
/// <param name="state">The state that user should have.</param>
public sealed class CallbackQueryUserHasNumericStateFilter(string stateKeeperName, int state = default)
    : UserHasNumericStateFilter<CallbackQuery>((callbackQuery) => callbackQuery.From, stateKeeperName, state)
{
}
