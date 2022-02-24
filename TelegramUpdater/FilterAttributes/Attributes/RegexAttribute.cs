using System.Text.RegularExpressions;
using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes
{
    /// <summary>
    /// Create a regex filter.
    /// </summary>
    public sealed class RegexAttribute : AbstractFilterAttribute
    {
        /// <summary>
        /// Create a regex filter attribute.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="regexOptions"></param>
        /// <param name="catchCaption"></param>
        public RegexAttribute(string pattern, RegexOptions? regexOptions = default, bool? catchCaption = default)
        {
            CatchCaption = catchCaption;
            Pattern = pattern;
            RegexOptions = regexOptions;
        }

        internal string Pattern { get; init; }

        internal RegexOptions? RegexOptions { get; init; }

        internal bool? CatchCaption { get; init; }

        /// <inheritdoc/>
        protected internal override object GetFilterTypeOf(Type requestedType)
        {
            if (requestedType == null)
                throw new ArgumentNullException(nameof(requestedType));

            if (requestedType == typeof(Message))
            {
                return new MessageTextRegex(Pattern, CatchCaption ?? false, RegexOptions);
            }
            else if (requestedType == typeof(CallbackQuery))
            {
                return new CallbackQueryRegex(Pattern, RegexOptions);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Regex attribute is not supported for {requestedType}");
            }
        }
    }
}
