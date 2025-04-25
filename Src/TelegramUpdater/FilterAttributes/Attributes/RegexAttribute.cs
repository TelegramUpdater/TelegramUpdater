using System.Text.RegularExpressions;
using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// Create a regex filter.
/// </summary>
/// <remarks>
/// Create a regex filter attribute.
/// </remarks>
/// <param name="pattern"></param>
/// <param name="regexOptions"></param>
/// <param name="catchCaption"></param>
public sealed class RegexAttribute(string pattern, RegexOptions regexOptions = default, bool catchCaption = false) : AbstractFilterAttribute
{
    internal string Pattern { get; init; } = pattern;

    internal RegexOptions? RegexOptions { get; init; } = regexOptions;

    internal bool? CatchCaption { get; init; } = catchCaption;

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
            return new MessageTextRegex(Pattern, CatchCaption ?? false, RegexOptions);
        }

        if (requestedType == typeof(CallbackQuery))
        {
            return new CallbackQueryRegex(Pattern, RegexOptions);
        }

        throw new InvalidOperationException(
            $"Regex attribute is not supported for {requestedType}");
    }
}
