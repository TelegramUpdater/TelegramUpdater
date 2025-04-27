// Ignore Spelling: Usernames Inline

namespace TelegramUpdater.Filters;

internal class FromUsernamesFilter<T> : UpdaterFilter<T> where T : class
{
    private readonly Func<T, string?> _usernameSelector;

    public string[] Usernames { get; }

    internal FromUsernamesFilter(
        Func<T, string?> usernameSelector, params string[] usernames)
    { 
        _usernameSelector = usernameSelector;
        Usernames = usernames;
    }

    public override bool TheyShellPass(UpdaterFilterInputs<T> inputs)
    {
        var username = _usernameSelector(inputs.Input);
        if (username == null) return false;

        if (Usernames.Any(x => string.Equals(x, username, StringComparison.OrdinalIgnoreCase)))
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
    public static UpdaterFilter<Message> Messages(params string[] usernames)
        => new FromUsernamesFilter<Message>(x => x.From?.Username, usernames);

    /// <summary>
    /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="CallbackQuery"/> handlers
    /// </summary>
    /// <param name="usernames">A list of usernames without @.</param>
    public static UpdaterFilter<CallbackQuery> CallbackQueries(params string[] usernames)
        => new FromUsernamesFilter<CallbackQuery>(x => x.From.Username, usernames);

    /// <summary>
    /// Create a <see cref="FromUsernamesFilter{T}"/> for <see cref="InlineQuery"/> handlers
    /// </summary>
    /// <param name="usernames">A list of usernames without @.</param>
    public static UpdaterFilter<InlineQuery> InlineQueries(params string[] usernames)
        => new FromUsernamesFilter<InlineQuery>(x => x.From.Username, usernames);
}
