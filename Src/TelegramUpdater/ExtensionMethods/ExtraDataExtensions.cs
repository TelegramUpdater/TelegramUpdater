using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TelegramUpdater.Filters;
using TelegramUpdater.Helpers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension method related to extra data.
/// </summary>
public static class ExtraDataExtensions
{
    // TODO: add extra data for regex matches

    /// <summary>
    /// Get <see cref="MatchCollection"/> exists in matches
    /// key of <see cref="IContainer"/>. in case of using regex filter.
    /// </summary>
    public static bool TryGetMatchCollection(
        this IContainer container, [NotNullWhen(true)] out MatchCollection? collection)
        => container.TryGetExtraData("matches", out collection);

    /// <summary>
    /// Tries to get and parse the command args. you should have a
    /// <see cref="CommandFilter"/>..
    /// </summary>
    public static bool TryParseCommandArgs<T1>(
        this IContainer container,
        [NotNullWhen(true)] out T1? output1)
    {
        if (container.TryGetExtraData("args", out string[]? commandArgs))
        {
            if (commandArgs.TryConvertArgs(out output1))
            {
                return true;
            }
        }
        else
        {
            throw new InvalidOperationException(
                "Command args not found. do you have any CommandFilters there?"
            );
        }

        output1 = default;
        return false;
    }

    /// <summary>
    /// Tries to get and parse the command args. you should have a
    /// <see cref="CommandFilter"/>..
    /// </summary>
    public static bool TryParseCommandArgs<T1, T2>(
        this IContainer container,
        [NotNullWhen(true)] out T1? output1,
        [NotNullWhen(true)] out T2? output2)
    {
        if (container.TryGetExtraData("args", out string[]? commandArgs))
        {
            if (commandArgs.TryConvertArgs(out output1, out output2))
            {
                return true;
            }
        }
        else
        {
            throw new InvalidOperationException(
                "Command args not found. do you have any CommandFilters there?"
            );
        }

        output1 = default;
        output2 = default;
        return false;
    }

    /// <summary>
    /// Tries to get and parse the command args. you should have a
    /// <see cref="CommandFilter"/>..
    /// </summary>
    public static bool TryParseCommandArgs<T1, T2, T3>(
        this IContainer container,
        [NotNullWhen(true)] out T1? output1,
        [NotNullWhen(true)] out T2? output2,
        [NotNullWhen(true)] out T3? output3)
    {
        if (container.TryGetExtraData("args", out string[]? commandArgs))
        {
            if (commandArgs.TryConvertArgs(out output1, out output2, out output3))
            {
                return true;
            }
        }
        else
        {
            throw new InvalidOperationException(
                "Command args not found. do you have any CommandFilters there?"
            );
        }

        output1 = default;
        output2 = default;
        output3 = default;
        return false;
    }

    /// <summary>
    /// Tries to get and parse the command args. you should have a
    /// <see cref="CommandFilter"/>..
    /// </summary>
    public static bool TryParseCommandArgs<T1, T2, T3, T4>(
        this IContainer container,
        [NotNullWhen(true)] out T1? output1,
        [NotNullWhen(true)] out T2? output2,
        [NotNullWhen(true)] out T3? output3,
        [NotNullWhen(true)] out T4? output4)
    {
        if (container.TryGetExtraData("args", out string[]? commandArgs))
        {
            if (commandArgs.TryConvertArgs(out output1, out output2, out output3, out output4))
            {
                return true;
            }
        }
        else
        {
            throw new InvalidOperationException(
                "Command args not found. do you have any CommandFilters there?"
            );
        }

        output1 = default;
        output2 = default;
        output3 = default;
        output4 = default;
        return false;
    }

}
