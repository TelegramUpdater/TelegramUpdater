using System.Text.RegularExpressions;

namespace TelegramUpdater.Filters;

/// <summary>
/// A basic regex filter.
/// </summary>
/// <remarks>
/// <b>Extra data:</b> <see cref="MatchCollection"/> "matches".
/// </remarks>
/// <typeparam name="T">Type to apply filter on.</typeparam>
public class BasicRegexFilter<T> : Filter<T>
{
    private readonly Func<T, string?> _getText;
    private readonly Regex _regex;

    /// <summary>
    /// Create a basic regex filter.
    /// </summary>
    /// <param name="getText">A function to extract text from <typeparamref name="T"/>.</param>
    /// <param name="pattern">Regex pattern.</param>
    /// <param name="regexOptions">Regex options.</param>
    /// <param name="matchTimeout">Timeout for regex matching.</param>
    public BasicRegexFilter(
        Func<T, string?> getText,
        string pattern,
        RegexOptions? regexOptions = default,
        TimeSpan? matchTimeout = default)
    {
        _getText = getText;
        _regex = new Regex(
            pattern, regexOptions ?? RegexOptions.None,
            matchTimeout ?? TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// Create a basic regex filter.
    /// </summary>
    /// <param name="getText">A function to extract text from <typeparamref name="T"/>.</param>
    /// <param name="regex">Regex compiled object.</param>
    public BasicRegexFilter(
        Func<T, string?> getText,
        Regex regex)
    {
        _getText = getText;
        _regex = regex;
    }

    /// <inheritdoc/>
    protected override bool TheyShellPass(T inputs)
    {
        var text = _getText(inputs);

        if (string.IsNullOrEmpty(text)) return false;

        var matches = _regex.Matches(text);

        if (matches.Count > 0)
        {
            AddOrUpdateData("matches", matches);
            return true;
        }

        return false;
    }
}

/// <summary>
/// A regex filter on any <see cref="string"/>.
/// </summary>
public sealed class StringRegex : BasicRegexFilter<string>
{
    /// <summary>
    /// Create an <see cref="string"/> regex filter.
    /// </summary>
    /// <param name="regex">Regex compiled object.</param>
    public StringRegex(Regex regex) : base(x => x, regex)
    {
    }

    /// <summary>
    /// Create an <see cref="string"/> regex filter.
    /// </summary>
    /// <param name="pattern">Regex pattern.</param>
    /// <param name="regexOptions">Regex options.</param>
    /// <param name="matchTimeout">Timeout for regex matching.</param>
    public StringRegex(string pattern, RegexOptions? regexOptions = null, TimeSpan? matchTimeout = null)
        : base(x => x, pattern, regexOptions, matchTimeout)
    {
    }
}

