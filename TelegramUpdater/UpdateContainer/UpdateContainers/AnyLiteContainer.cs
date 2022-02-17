using System;
using System.Linq.Expressions;
using System.Reflection;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    /// <summary>
    /// Lite contianer to use for updates that are catched outside of updaters.
    /// Eg: the result of requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnyLiteContainer<T> : UpdateContainerAbs<T> where T : class
    {
        /// <summary>
        /// Create a lite container.
        /// </summary>
        /// <param name="insiderResovler">A function to resolve inner update.</param>
        /// <param name="update">The update itself.</param>
        /// <param name="updater">The updater instanse</param>
        internal AnyLiteContainer(
            Func<Update, T?> insiderResovler,
            Update update,
            IUpdater updater) : base(insiderResovler, updater, update)
        { }


        /// <inheritdoc/>
        public override ShiningInfo<long, Update> ShiningInfo
            => throw new InvalidOperationException(
                "Lite contianers have no ShiningInfo, since they're not received from updater.");

        internal static IContainer<U> CreateLiteContainer<U>(
            Expression<Func<Update, U?>> insiderResovler, U update, IUpdater updater)
            where U : class
        {
            var u = new Update();
            var prop = (PropertyInfo)((MemberExpression)insiderResovler.Body).Member;
            prop.SetValue(u, update);

            return new AnyLiteContainer<U>(insiderResovler.Compile(), u, updater);
        }

        internal static IContainer<Message> MessageLiteContainer(Message update, IUpdater updater)
            => CreateLiteContainer(x=> x.Message, update, updater);

        internal static IContainer<CallbackQuery> CallbackQueryLiteContainer(CallbackQuery update, IUpdater updater)
            => CreateLiteContainer(x=> x.CallbackQuery, update, updater);
    }
}
