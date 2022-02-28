using System.Text.RegularExpressions;

namespace TelegramUpdater.Filters
{
    /// <summary>
    /// A basic regex filter.
    /// </summary>
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
                matchTimeout?? TimeSpan.FromSeconds(5));
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
            _getText= getText;
            _regex = regex;
        }

        /// <inheritdoc/>
        public override bool TheyShellPass(T input)
        {
            var text = _getText(input);

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
        public StringRegex(Regex regex) : base(x=> x, regex)
        {
        }

        /// <summary>
        /// Create an <see cref="string"/> regex filter.
        /// </summary>
        /// <param name="pattern">Regex pattern.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <param name="matchTimeout">Timeout for regex matching.</param>
        public StringRegex(string pattern, RegexOptions? regexOptions = null, TimeSpan? matchTimeout = null)
            : base(x=> x, pattern, regexOptions, matchTimeout)
        {
        }
    }

    /// <summary>
    /// A regex filter applied on <see cref="CallbackQuery"/>(es).
    /// </summary>
    public sealed class CallbackQueryRegex : BasicRegexFilter<CallbackQuery>
    {
        /// <summary>
        /// Create an <see cref="CallbackQuery.Data"/> regex filter.
        /// </summary>
        /// <param name="regex">Regex compiled object.</param>
        public CallbackQueryRegex(Regex regex) : base(x=> x.Data, regex)
        {
        }

        /// <summary>
        /// Create an <see cref="CallbackQuery.Data"/> regex filter.
        /// </summary>
        /// <param name="pattern">Regex pattern.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <param name="matchTimeout">Timeout for regex matching.</param>
        public CallbackQueryRegex(string pattern, RegexOptions? regexOptions = null, TimeSpan? matchTimeout = null)
            : base(x=> x.Data, pattern, regexOptions, matchTimeout)
        {
        }
    }

    /// <summary>
    /// A regex filter applied on <see cref="Message"/>s.
    /// </summary>
    public sealed class MessageTextRegex : BasicRegexFilter<Message>
    {
        /// <summary>
        /// Create an <see cref="Message"/> regex filter.
        /// </summary>
        /// <param name="catchCaption">If the caption can be included.</param>
        /// <param name="regex">Regex compiled object.</param>
        public MessageTextRegex(Regex regex, bool catchCaption = false)
            : base(x =>
            {
                return x switch
                {
                    { Text: { } text } => text,
                    { Caption: { } caption } when catchCaption => caption,
                    _ => null
                };
            }, regex)
        {
        }

        /// <summary>
        /// Create an <see cref="Message"/> regex filter.
        /// </summary>
        /// <param name="pattern">Regex pattern.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <param name="matchTimeout">Timeout for regex matching.</param>
        /// <param name="catchCaption">If the caption can be included.</param>
        public MessageTextRegex(
            string pattern,
            bool catchCaption = false,
            RegexOptions? regexOptions = null,
            TimeSpan? matchTimeout = null)
            : base(x =>
            {
                return x switch
                {
                    { Text: { } text } => text,
                    { Caption: { } caption } when catchCaption => caption,
                    _ => null
                };
            }, pattern, regexOptions, matchTimeout)
        {
        }
    }
}
