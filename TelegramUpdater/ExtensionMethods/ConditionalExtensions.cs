using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater
{
    public static class ConditionalExtensions
    {
        #region Sync

        #region Abstracts
        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static MatchContext<T> If<T>(this UpdateContainerAbs<T> simpleContext,
                                            Func<T, string?> getText,
                                            string pattern,
                                            Action<UpdateContainerAbs<T>> func,
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
        public static MatchContext<T> If<T>(this UpdateContainerAbs<T> simpleContext,
                                            Func<UpdateContainerAbs<T>, bool> predict,
                                            Action<UpdateContainerAbs<T>> func) where T : class
        {
            if (predict(simpleContext))
            {
                func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        /// <summary>
        /// Do something when a regex not matched.
        /// </summary>
        public static void Else<T>(this MatchContext<T> matchContext,
                                   Action<UpdateContainerAbs<T>> func) where T : class
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
        public static MatchContext<T> ElseIf<T>(this MatchContext<T> matchContext,
                                                Func<T, string?> getText,
                                                string pattern,
                                                Action<UpdateContainerAbs<T>> func,
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
        public static MatchContext<T> ElseIf<T>(this MatchContext<T> matchContext,
                                                Func<UpdateContainerAbs<T>, bool> predict,
                                                Action<UpdateContainerAbs<T>> func) where T : class
        {
            if (!matchContext)
            {
                if (predict(matchContext.SimpleContext))
                {
                    func(matchContext.SimpleContext);
                    return new MatchContext<T>(matchContext.SimpleContext, true);
                }

                return default;
            }

            return matchContext;
        }

        public static MatchContext<T> IfNotNull<T>(this UpdateContainerAbs<T>? simpleContext,
                                                   Action<UpdateContainerAbs<T>> func) where T : class
        {
            if (simpleContext != null)
            {
                func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        public static void IfNotNull<T>(this T everything, Action<T> action)
            where T : class
        {
            if (everything != null)
            {
                action(everything);
            }
        }

        #endregion

        #endregion

        #region Async

        #region Abstracts
        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this UpdateContainerAbs<T> simpleContext,
                                                        Func<T, string?> getText,
                                                        string pattern,
                                                        Func<UpdateContainerAbs<T>, Task> func,
                                                        RegexOptions? regexOptions = default) where T : class
        {
            var match = MatchContext<T>.Check(simpleContext, getText, pattern, regexOptions);

            if (match)
            {
                await func(simpleContext);
            }

            return match;
        }

        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this Task<UpdateContainerAbs<T>> simpleContext,
                                                        Func<T, string?> getText,
                                                        string pattern,
                                                        Func<UpdateContainerAbs<T>, Task> func,
                                                        RegexOptions? regexOptions = default) where T : class
        {
            var gottenContext = await simpleContext;

            var match = MatchContext<T>.Check(gottenContext, getText, pattern, regexOptions);

            if (match)
            {
                await func(gottenContext);
            }

            return match;
        }

        /// <summary>
        /// Do something when a condition is true.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this UpdateContainerAbs<T> simpleContext,
                                                        Func<UpdateContainerAbs<T>, bool> predict,
                                                        Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            if (predict(simpleContext))
            {
                await func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        /// <summary>
        /// Do something when a condition is true.
        /// </summary>
        public static async Task<MatchContext<T>> If<T>(this Task<UpdateContainerAbs<T>> simpleContext,
                                                        Func<UpdateContainerAbs<T>, bool> predict,
                                                        Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var gottenContext = await simpleContext;

            if (predict(gottenContext))
            {
                await func(gottenContext);
                return new MatchContext<T>(gottenContext, true);
            }

            return default;
        }

        /// <summary>
        /// If this <see cref="UpdateContainerAbs{T}"/> is not null
        /// </summary>
        public static async Task<MatchContext<T>> IfNotNull<T>(this UpdateContainerAbs<T>? simpleContext,
                                                               Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            if (simpleContext != null)
            {
                await func(simpleContext);
                return new MatchContext<T>(simpleContext, true);
            }

            return default;
        }

        /// <summary>
        /// If this <see cref="UpdateContainerAbs{T}"/> is not null
        /// </summary>
        public static async Task<MatchContext<T>> IfNotNull<T>(this Task<UpdateContainerAbs<T>?> simpleContext,
                                                               Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var gottenContext = await simpleContext;

            if (gottenContext != null)
            {
                await func(gottenContext);
                return new MatchContext<T>(gottenContext, true);
            }

            return default;
        }

        /// <summary>
        /// Do something when a regex not matched.
        /// </summary>
        public static async Task Else<T>(this Task<MatchContext<T>> matchContext,
                                         Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var match = await matchContext;
            if (!match)
            {
                await func(match.SimpleContext);
            }
        }

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<T>> ElseIf<T>(this Task<MatchContext<T>> matchContext,
                                                            Func<T, string?> getText,
                                                            string pattern,
                                                            Func<UpdateContainerAbs<T>, Task> func,
                                                            RegexOptions? regexOptions = default) where T : class
        {
            var prevMatch = await matchContext;
            if (!prevMatch)
            {
                var match = MatchContext<T>.Check(prevMatch.SimpleContext, getText, pattern, regexOptions);

                if (match)
                {
                    await func(prevMatch.SimpleContext);
                }

                return match;
            }

            return prevMatch;
        }

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<T>> ElseIf<T>(this Task<MatchContext<T>> matchContext,
                                                            Func<UpdateContainerAbs<T>, bool> predict,
                                                            Func<UpdateContainerAbs<T>, Task> func) where T : class
        {
            var prevMatch = await matchContext;
            if (!prevMatch)
            {
                if (predict(prevMatch.SimpleContext))
                {
                    await func(prevMatch.SimpleContext);
                    return new MatchContext<T>(prevMatch.SimpleContext, true);
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
        public static async Task<MatchContext<Message>> If(this UpdateContainerAbs<Message> simpleContext,
                                                                  string pattern,
                                                                  Func<UpdateContainerAbs<Message>, Task> func,
                                                                  RegexOptions? regexOptions = default)
            => await simpleContext.If(x => x.Text, pattern, func, regexOptions);

        /// <summary>
        /// Do something when a regex matched.
        /// </summary>
        public static async Task<MatchContext<CallbackQuery>> If(this UpdateContainerAbs<CallbackQuery> simpleContext,
                                                                        string pattern,
                                                                        Func<UpdateContainerAbs<CallbackQuery>, Task> func,
                                                                        RegexOptions? regexOptions = default)
            => await simpleContext.If(x => x.Data, pattern, func, regexOptions);

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<CallbackQuery>> ElseIf(this Task<MatchContext<CallbackQuery>> matchContext,
                                                                     string pattern,
                                                                     Func<UpdateContainerAbs<CallbackQuery>, Task> func,
                                                                     RegexOptions? regexOptions = default)
            => await matchContext.ElseIf(x => x.Data, pattern, func, regexOptions);

        /// <summary>
        /// Do something when a regex not matched but something else matched.
        /// </summary>
        public static async Task<MatchContext<Message>> ElseIf(this Task<MatchContext<Message>> matchContext,
                                                               string pattern,
                                                               Func<UpdateContainerAbs<Message>, Task> func,
                                                               RegexOptions? regexOptions = default)
            => await matchContext.ElseIf(x => x.Text, pattern, func, regexOptions);

        #endregion

        #endregion
    }
}
