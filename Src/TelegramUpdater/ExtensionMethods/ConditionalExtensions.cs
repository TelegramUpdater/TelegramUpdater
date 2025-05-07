using System.Text.RegularExpressions;
using TelegramUpdater.Helpers;

namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// Some extension methods to handle conditional stuff.
/// </summary>
public static class ConditionalExtensions
{
    #region Sync

    #region Abstracts
    /// <summary>
    /// Do something when a regex matched.
    /// </summary>
    public static MatchContext<T> If<T>(
        this IContainer<T> simpleContext,
        Func<T, string?> getText,
        string pattern,
        Action<IContainer<T>> func,
        RegexOptions? regexOptions = default) where T : class
    {
        var match = MatchContext<T>.Check(simpleContext, getText, pattern, regexOptions);

        if (match)
        {
            func(simpleContext);
        }

        return match;
    }

    /// <summary>
    /// Do something when a condition is true.
    /// </summary>
    public static MatchContext<T> If<T>(
        this IContainer<T> simpleContext,
        Func<IContainer<T>, bool> predict,
        Action<IContainer<T>> func) where T : class
    {
        if (predict(simpleContext))
        {
            func(simpleContext);
            return new MatchContext<T>(simpleContext, isMatched: true);
        }

        return default;
    }

    /// <summary>
    /// Do something when a regex not matched.
    /// </summary>
    public static void Else<T>(
        this MatchContext<T> matchContext,
        Action<IContainer<T>> func) where T : class
    {
        var match = matchContext;
        if (!match)
        {
            func(match.SimpleContext);
        }
    }

    /// <summary>
    /// Do something when a regex not matched but something else matched.
    /// </summary>
    public static MatchContext<T> ElseIf<T>(
        this MatchContext<T> matchContext,
        Func<T, string?> getText,
        string pattern,
        Action<IContainer<T>> func,
        RegexOptions? regexOptions = default) where T : class
    {
        if (!matchContext)
        {
            var match = MatchContext<T>.Check(matchContext.SimpleContext, getText, pattern, regexOptions);

            if (match)
            {
                func(matchContext.SimpleContext);
            }

            return match;
        }

        return matchContext;
    }

    /// <summary>
    /// Do something when a regex not matched but something else matched.
    /// </summary>
    public static MatchContext<T> ElseIf<T>(
        this MatchContext<T> matchContext,
        Func<IContainer<T>, bool> predict,
        Action<IContainer<T>> func) where T : class
    {
        if (!matchContext)
        {
            if (predict(matchContext.SimpleContext))
            {
                func(matchContext.SimpleContext);
                return new MatchContext<T>(matchContext.SimpleContext, isMatched: true);
            }

            return default;
        }

        return matchContext;
    }

    /// <summary>
    /// Do something is something else is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="simpleContext"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static MatchContext<T> IfNotNull<T>(
        this IContainer<T>? simpleContext,
        Action<IContainer<T>> func) where T : class
    {
        if (simpleContext != null)
        {
            func(simpleContext);
            return new MatchContext<T>(simpleContext, isMatched: true);
        }

        return default;
    }

    /// <summary>
    /// Do something is something else is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="anything"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static void IfNotNull<T>(this T anything, Action<T> action)
        where T : class
    {
        if (anything != null)
        {
            action(anything);
        }
    }

    #endregion

    #endregion

    #region Async

    #region Abstracts
    /// <summary>
    /// Do something when a regex matched.
    /// </summary>
    public static async Task<MatchContext<T>> If<T>(
        this IContainer<T> simpleContext,
        Func<T, string?> getText,
        string pattern,
        Func<IContainer<T>, Task> func,
        RegexOptions? regexOptions = default) where T : class
    {
        var match = MatchContext<T>.Check(simpleContext, getText, pattern, regexOptions);

        if (match)
        {
            await func(simpleContext).ConfigureAwait(false);
        }

        return match;
    }

    /// <summary>
    /// Do something when a regex matched.
    /// </summary>
    public static async Task<MatchContext<T>> If<T>(
        this Task<IContainer<T>> simpleContext,
        Func<T, string?> getText,
        string pattern,
        Func<IContainer<T>, Task> func,
        RegexOptions? regexOptions = default) where T : class
    {
        var gottenContext = await simpleContext.ConfigureAwait(false);

        var match = MatchContext<T>.Check(gottenContext, getText, pattern, regexOptions);

        if (match)
        {
            await func(gottenContext).ConfigureAwait(false);
        }

        return match;
    }

    /// <summary>
    /// Do something when a condition is true.
    /// </summary>
    public static async Task<MatchContext<T>> If<T>(
        this IContainer<T> simpleContext,
        Func<IContainer<T>, bool> predict,
        Func<IContainer<T>, Task> func) where T : class
    {
        if (predict(simpleContext))
        {
            await func(simpleContext).ConfigureAwait(false);
            return new MatchContext<T>(simpleContext, isMatched: true);
        }

        return default;
    }

    /// <summary>
    /// Do something when a condition is true.
    /// </summary>
    public static async Task<MatchContext<T>> If<T>(
        this Task<IContainer<T>> simpleContext,
        Func<IContainer<T>, bool> predict,
        Func<IContainer<T>, Task> func) where T : class
    {
        var gottenContext = await simpleContext.ConfigureAwait(false);

        if (predict(gottenContext))
        {
            await func(gottenContext).ConfigureAwait(false);
            return new MatchContext<T>(gottenContext, isMatched:  true);
        }

        return default;
    }

    /// <summary>
    /// If this <see cref="AbstractUpdateContainer{T}"/> is not null
    /// </summary>
    public static async Task<MatchContext<T>> IfNotNull<T>(
        this IContainer<T>? simpleContext,
        Func<IContainer<T>, Task> func) where T : class
    {
        if (simpleContext != null)
        {
            await func(simpleContext).ConfigureAwait(false);
            return new MatchContext<T>(simpleContext, isMatched: true);
        }

        return default;
    }

    /// <summary>
    /// If this <see cref="AbstractUpdateContainer{T}"/> is not null
    /// </summary>
    public static async Task<MatchContext<T>> IfNotNull<T>(
        this Task<IContainer<T>?> simpleContext,
        Func<IContainer<T>, Task> func) where T : class
    {
        var gottenContext = await simpleContext.ConfigureAwait(false);

        if (gottenContext != null)
        {
            await func(gottenContext).ConfigureAwait(false);
            return new MatchContext<T>(gottenContext, isMatched: true);
        }

        return default;
    }

    /// <summary>
    /// Do something when a regex not matched.
    /// </summary>
    public static async Task Else<T>(
        this Task<MatchContext<T>> matchContext,
        Func<IContainer<T>, Task> func) where T : class
    {
        var match = await matchContext.ConfigureAwait(false);
        if (!match)
        {
            await func(match.SimpleContext).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Do something when a regex not matched but something else matched.
    /// </summary>
    public static async Task<MatchContext<T>> ElseIf<T>(
        this Task<MatchContext<T>> matchContext,
        Func<T, string?> getText,
        string pattern,
        Func<IContainer<T>, Task> func,
        RegexOptions? regexOptions = default) where T : class
    {
        var prevMatch = await matchContext.ConfigureAwait(false);
        if (!prevMatch)
        {
            var match = MatchContext<T>.Check(prevMatch.SimpleContext, getText, pattern, regexOptions);

            if (match)
            {
                await func(prevMatch.SimpleContext).ConfigureAwait(false);
            }

            return match;
        }

        return prevMatch;
    }

    /// <summary>
    /// Do something when a regex not matched but something else matched.
    /// </summary>
    public static async Task<MatchContext<T>> ElseIf<T>(
        this Task<MatchContext<T>> matchContext,
        Func<IContainer<T>, bool> predict,
        Func<IContainer<T>, Task> func) where T : class
    {
        var prevMatch = await matchContext.ConfigureAwait(false);
        if (!prevMatch)
        {
            if (predict(prevMatch.SimpleContext))
            {
                await func(prevMatch.SimpleContext).ConfigureAwait(false);
                return new MatchContext<T>(prevMatch.SimpleContext, isMatched: true);
            }

            return default;
        }

        return prevMatch;
    }

    #endregion

    #region Sealed

    /// <summary>
    /// Do something when a regex matched.
    /// </summary>
    public static async Task<MatchContext<Message>> If(
        this IContainer<Message> simpleContext,
        string pattern,
        Func<IContainer<Message>, Task> func,
        RegexOptions? regexOptions = default)
        => await simpleContext.If(x => x.Text, pattern, func, regexOptions).ConfigureAwait(false);

    /// <summary>
    /// Do something when a regex matched.
    /// </summary>
    public static async Task<MatchContext<CallbackQuery>> If(
        this IContainer<CallbackQuery> simpleContext,
        string pattern,
        Func<IContainer<CallbackQuery>, Task> func,
        RegexOptions? regexOptions = default)
        => await simpleContext.If(x => x.Data, pattern, func, regexOptions).ConfigureAwait(false);

    /// <summary>
    /// Do something when a regex not matched but something else matched.
    /// </summary>
    public static async Task<MatchContext<CallbackQuery>> ElseIf(
        this Task<MatchContext<CallbackQuery>> matchContext,
        string pattern,
        Func<IContainer<CallbackQuery>, Task> func,
        RegexOptions? regexOptions = default)
        => await matchContext.ElseIf(x => x.Data, pattern, func, regexOptions).ConfigureAwait(false);

    /// <summary>
    /// Do something when a regex not matched but something else matched.
    /// </summary>
    public static async Task<MatchContext<Message>> ElseIf(
        this Task<MatchContext<Message>> matchContext,
        string pattern,
        Func<IContainer<Message>, Task> func,
        RegexOptions? regexOptions = default)
        => await matchContext.ElseIf(x => x.Text, pattern, func, regexOptions).ConfigureAwait(false);

    #endregion

    #endregion
}
