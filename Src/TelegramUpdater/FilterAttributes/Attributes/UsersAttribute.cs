// Ignore Spelling: usernames

using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// An attribute to filter users based on their id or username.
/// </summary>
public sealed class UsersAttribute : AbstractFilterAttribute
{
    [MemberNotNullWhen(true, nameof(UserIds))]
    [MemberNotNullWhen(false, nameof(Usernames))]
    private bool OnIds { get; }

    /// <summary>
    /// Creates a <see cref="FromUsernamesFilter{T}"/> attribute.
    /// </summary>
    /// <param name="usernames">Usernames without @.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public UsersAttribute(params string[]? usernames)
    {
        if (usernames == null || usernames.Length == 0)
            throw new ArgumentNullException(nameof(usernames));
        foreach (var user in usernames)
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException("One of your usernames is null or empty.", nameof(usernames));

        Usernames = usernames;
        OnIds = false;
    }

    /// <summary>
    /// Creates a <see cref="FromUsersFilter{T}"/> attribute.
    /// </summary>
    /// <param name="userIds">Userids.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public UsersAttribute(params long[]? userIds)
    {
        if (userIds == null || userIds.Length == 0)
            throw new ArgumentNullException(nameof(userIds));

        OnIds = true;
        UserIds = userIds;
    }

    internal long[]? UserIds { get; init; }

    internal string[]? Usernames { get; init; }

    /// <inheritdoc/>
    protected internal override object GetFilterTypeOf(Type requestedType)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(requestedType);
#else
        if (requestedType is null)
            throw new ArgumentNullException(nameof(requestedType));
#endif

        if (requestedType == typeof(Message))
        {
            return OnIds ? FromUsersFilter.Messages(UserIds) :
                FromUsernamesFilter.Messages(Usernames);
        }

        if (requestedType == typeof(CallbackQuery))
        {
            return OnIds ? FromUsersFilter.CallbackQueries(UserIds) :
                FromUsernamesFilter.CallbackQueries(Usernames);
        }

        return requestedType == typeof(InlineQuery)
            ? (object)(OnIds ? FromUsersFilter.InlineQueries(UserIds) :
                        FromUsernamesFilter.InlineQueries(Usernames))
            : throw new ArgumentException(
                        $"Users filter attribute is not supported for {requestedType}", nameof(requestedType));
    }
}
