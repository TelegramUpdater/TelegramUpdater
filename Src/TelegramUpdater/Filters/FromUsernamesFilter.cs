namespace TelegramUpdater.Filters;

internal class FromUsernamesFilter<T> : Filter<T> where T : class
{
    private readonly Func<T, string?> _usernameSelector;

    public string[] Usernames { get; }

    internal FromUsernamesFilter(
        Func<T, string?> usernameSelector, params string[] usernames)
    { 
        _usernameSelector = usernameSelector;
        Usernames = usernames;
    }

    public override bool TheyShellPass(IUpdater _, T input)
    {
        var username = _usernameSelector(input);
        if (username == null) return false;

        if (Usernames.Any(x => x == username))
        {
            AddOrUpdateData("username", username);
            return true;
        }
        return false;
    }
}

/// <summary>
/// Use this to create a <see cref="FromUsernamesFilter{T}"/>. where is Update type.
/// </summary>
/// <remarks>
/// <b>Extra data:</b> <see cref="string"/> "username".
/// </remarks>
public static class FromUsernamesFilter
{
    /// <summary>
    /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="Message"/> handlers
    /// </summary>
    /// <param name="usernames">A list of usernames without @.</param>
    public static Filter<Message> Messages(params string[] usernames)
        => new FromUsernamesFilter<Message>(x => x.From?.Username, usernames);

    /// <summary>
    /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="CallbackQuery"/> handlers
    /// </summary>
    /// <param name="usernames">A list of usernames without @.</param>
    public static Filter<CallbackQuery> CallbackQueries(params string[] usernames)
        => new FromUsernamesFilter<CallbackQuery>(x => x.From.Username, usernames);

    /// <summary>
    /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="InlineQuery"/> handlers
    /// </summary>
    /// <param name="usernames">A list of usernames without @.</param>
    public static Filter<InlineQuery> InlineQueries(params string[] usernames)
        => new FromUsernamesFilter<InlineQuery>(x => x.From.Username, usernames);
}
