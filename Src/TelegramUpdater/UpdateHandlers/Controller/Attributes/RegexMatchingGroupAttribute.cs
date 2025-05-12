using System.Text.RegularExpressions;
using TelegramUpdater.Helpers;

namespace TelegramUpdater.UpdateHandlers.Controller.Attributes;

internal enum RegexGroupIndexTarget
{
    Num = 0,
    Name,
}

/// <summary>
/// Regex group name or index (not both).
/// </summary>
public class RegexGroupIndex
{
    private RegexGroupIndex() { }

    /// <summary>
    /// Unnamed regex group.
    /// </summary>
    /// <param name="groupNum"></param>
    public RegexGroupIndex(int groupNum)
    {
        Target = RegexGroupIndexTarget.Num;
        GroupNum = groupNum;
    }

    /// <summary>
    /// Named regex group.
    /// </summary>
    /// <param name="groupName"></param>
    public RegexGroupIndex(string groupName)
    {
        Target = RegexGroupIndexTarget.Name;
        GroupName = groupName;
    }

    internal RegexGroupIndexTarget Target { get; }

    internal int GroupNum { get; }

    internal string? GroupName { get; }

    /// <summary>
    /// Unnamed regex group.
    /// </summary>
    public static implicit operator RegexGroupIndex(int groupNum) => new(groupNum);

    /// <summary>
    /// Named regex group.
    /// </summary>
    public static implicit operator RegexGroupIndex(string groupName) => new(groupName);
}

/// <summary>
/// Mark a parameter as its value should be acquired from regex's named matching group.
/// </summary>
public class RegexMatchingGroupAttribute : ExtraDataAttribute
{
    /// <summary>
    /// The index of match in <see cref="MatchCollection"/>,
    /// which is mostly the first matching group, unless you know what you're doing.
    /// </summary>
    public int MatchIndex { get; set; } = 0;

    /// <summary>
    /// Named or unnamed (index based) regex group.
    /// </summary>
    public RegexGroupIndex Group { get; }

    /// <param name="group">
    /// Unnamed (index based) regex group
    /// </param>
    public RegexMatchingGroupAttribute(int group) : base("matches")
    {
        Group = group;
    }

    /// <param name="group">
    /// Named regex group.
    /// </param>
    public RegexMatchingGroupAttribute(string group) : base("matches")
    {
        Group = group;
    }

    /// <inheritdoc/>
    protected internal override bool Polish(Type type, object? input, out object? output)
    {
        if (input is MatchCollection collection)
        {
            if (collection.Count > MatchIndex)
            {
                var match = collection[MatchIndex];
                if (match.Groups.Count > 0)
                {
                    // Check if the group is named or unnamed
                    if (Group.Target == RegexGroupIndexTarget.Num)
                    {
                        // Unnamed group
                        if (match.Groups.Count > Group.GroupNum)
                        {
                            var _group = match.Groups[Group.GroupNum];
                            if (_group.Success)
                            {
                                // Try to convert the argument to the type of the parameter
                                if (_group.Value.TryConvert(type, out output))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else
                    {
#if NET8_0_OR_GREATER
                        var exists = match.Groups.ContainsKey(Group.GroupName!);
#else
                        var exists = match.Groups.Any(
                            x => string.Equals(x.Name, Group.GroupName, StringComparison.Ordinal));
#endif
                        if (exists)
                        {
                            var _group = match.Groups[Group.GroupName!];
                            if (_group.Success)
                            {
                                // Try to convert the argument to the type of the parameter
                                if (_group.Value.TryConvert(type, out output))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }

        output = default;
        return false;
    }
}
