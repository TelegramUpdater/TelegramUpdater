using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.Helpers
{
    /// <summary>
    /// Provide information of a regex match.
    /// </summary>
    public readonly struct MatchContext<TUpdate> where TUpdate : class
    {
        internal MatchContext(IContainer<TUpdate> simpleContext,
                            bool isMatched = default,
                            MatchCollection? matchCollection = default)
        {
            SimpleContext = simpleContext;
            IsMatched = isMatched;
            MatchCollection = matchCollection;
        }

        /// <summary>
        /// Indicates if the regex is matched.
        /// </summary>
        [MemberNotNullWhen(true, "MatchCollection")]
        public bool IsMatched { get; }

        /// <summary>
        /// A <see cref="MatchCollection"/> if <see cref="IsMatched"/> is
        /// <see langword="true"/>.
        /// </summary>
        public MatchCollection? MatchCollection { get; }

        /// <summary>
        /// The update container.
        /// </summary>
        public IContainer<TUpdate> SimpleContext { get; }

        internal static MatchContext<T> Check<T>(
            IContainer<T> simpleContext,
            Func<T, string?> getText,
            string pattern,
            RegexOptions? regexOptions = default) where T : class
        {
            var text = getText(simpleContext.Update);

            if (string.IsNullOrEmpty(text)) return new MatchContext<T>(simpleContext);

            var matches = Regex.Matches(
                text, pattern, regexOptions ?? RegexOptions.None, TimeSpan.FromSeconds(3));

            if (matches.Count > 0)
            {
                return new MatchContext<T>(simpleContext, true, matches);
            }

            return new MatchContext<T>(simpleContext);
        }

        /// <summary>
        /// The result of <see cref="IsMatched"/>. 
        /// </summary>
        /// <param name="matchContext"></param>
        public static implicit operator bool(MatchContext<TUpdate> matchContext)
            => matchContext.IsMatched;
    }
}
