using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater
{
    public static class ScopedHandlersExtensions
    {
        /// <summary>
        /// Adds an scoped <see cref="Message"/> handler to the <paramref name="updater"/>.
        /// </summary>
        /// <typeparam name="THandler">Your <see cref="Message"/> handler type.</typeparam>
        /// <param name="updater"><see cref="Updater"/> itself.</param>
        /// <param name="filter">
        /// The filter to choose the right updates to handle.
        /// <para>
        /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
        /// </para>
        /// </param>
        public static void AddScopedMessage<THandler>(this Updater updater,
                                                      Filter<Message>? filter = default)
            where THandler : IScopedUpdateHandler
            => updater.AddScopedHandler<THandler, Message>(
                filter, UpdateType.Message, x => x.Message!);

        /// <summary>
        /// Adds an scoped <see cref="CallbackQuery"/> handler to the <paramref name="updater"/>.
        /// </summary>
        /// <typeparam name="THandler">Your <see cref="CallbackQuery"/> handler type.</typeparam>
        /// <param name="updater"><see cref="Updater"/> itself.</param>
        /// <param name="filter">
        /// The filter to choose the right updates to handle.
        /// <para>
        /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
        /// </para>
        /// </param>
        public static void AddScopedCallbackQuery<THandler>(this Updater updater,
                                                            Filter<CallbackQuery>? filter = default)
            where THandler : IScopedUpdateHandler
            => updater.AddScopedHandler<THandler, CallbackQuery>(
                filter, UpdateType.Message, x => x.CallbackQuery!);
    }
}
