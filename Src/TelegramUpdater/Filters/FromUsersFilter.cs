namespace TelegramUpdater.Filters;

internal class FromUsersFilter<T> : UpdaterFilter<T>
{
    private readonly Func<T, long?> _userSelector;

    public long[] Users { get; }

    internal FromUsersFilter(Func<T, long?> userSelector, params long[] users)
    {
        _userSelector = userSelector;
        Users = users;
    }

    public override bool TheyShellPass(UpdaterFilterInputs<T> input)
    {
        var user = _userSelector(input.Input);
        if (user is null) return false;

        if (Users.Any(x => x == user))
        {
            AddOrUpdateData("userId", user);
            return true;
        }
        return false;
    }
}

/// <summary>
/// Use this to create a <see cref="FromUsersFilter{T}"/>. where is Update type.
/// </summary>
/// <remarks>
/// <b>Extra data:</b> <see cref="int"/> "userId".
/// </remarks>
public static class FromUsersFilter
{
    /// <summary>
    /// Create an instance of <see cref="FromUsersFilter{T}"/> for <see cref="Message"/> handlers.
    /// </summary>
    /// <param name="users">User ids</param>
    public static UpdaterFilter<Message> Messages(params long[] users)
        => new FromUsersFilter<Message>(x => x.From?.Id, users);

    /// <summary>
    /// Create an instance of <see cref="FromUsersFilter{T}"/> for <see cref="CallbackQuery"/> handlers.
    /// </summary>
    /// <param name="users">User ids</param>
    public static UpdaterFilter<CallbackQuery> CallbackQueries(params long[] users)
        => new FromUsersFilter<CallbackQuery>(x => x.From.Id, users);

    /// <summary>
    /// Create an instance of <see cref="FromUsersFilter{T}"/> for <see cref="InlineQuery"/> handlers.
    /// </summary>
    /// <param name="users">User ids</param>
    public static UpdaterFilter<InlineQuery> InlineQueries(params long[] users)
        => new FromUsersFilter<InlineQuery>(x => x.From.Id, users);
}
