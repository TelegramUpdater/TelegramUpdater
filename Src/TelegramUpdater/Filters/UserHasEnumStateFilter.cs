namespace TelegramUpdater.Filters;

/// <summary>
/// Filter user to have the specified state of type <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">The enum state</typeparam>
/// <typeparam name="T">The target update.</typeparam>
/// <param name="userResolver">A function to resolve user from update.</param>
/// <param name="stateKeeperName">The name of state keeper. Must be added to the updater before.</param>
/// <param name="state">The specified state.</param>
public class UserHasEnumStateFilter<TEnum, T>(Func<T, User?> userResolver, string stateKeeperName, TEnum state = default)
    : Filter<T>((updater, update) =>
        {
            var keeper = updater.GetUserEnumStateKeeper<TEnum>(stateKeeperName);
            var user = userResolver(update);

            if (user is null) return false;

            return keeper.HasState(user, state);
        })
    where T : class
    where TEnum: struct, Enum
{
}

/// <inheritdoc />
public sealed class MessageUserHasEnumStateFilter<TEnum>(string stateKeeperName, TEnum state = default)
    : UserHasEnumStateFilter<TEnum, Message>((message) => message.From, stateKeeperName, state)
    where TEnum : struct, Enum
{
}

/// <inheritdoc />
public sealed class CallbackQueryUserHasEnumStateFilter<TEnum>(string stateKeeperName, TEnum state = default)
    : UserHasEnumStateFilter<TEnum, CallbackQuery>((message) => message.From, stateKeeperName, state)
    where TEnum : struct, Enum
{
}
