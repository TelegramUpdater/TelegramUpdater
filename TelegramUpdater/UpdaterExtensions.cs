using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater
{
    public static class CommonExtensions
    {
        internal static MessageContainer RebaseContainer<T>(
            this IContainer<T> containerAbs, Message message) where T : class
            => new MessageContainer(
                containerAbs.Updater,
                new Update { Message = message },
                containerAbs.BotClient);

        internal static MessageContainer Wrap<T>(this Message message,
                                            IContainer<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(message);

        internal static async Task<MessageContainer> WrapAsync<T>(this Task<Message> message,
                                            IContainer<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(await message);

        internal static CallbackQueryContainer RebaseContainer<T>(
            this IContainer<T> containerAbs, CallbackQuery callbackQuery) where T : class
            => new CallbackQueryContainer(
                containerAbs.Updater,
                new Update { CallbackQuery = callbackQuery },
                containerAbs.BotClient);

        internal static CallbackQueryContainer Wrap<T>(this CallbackQuery message,
                                            IContainer<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(message);

        internal static async Task<CallbackQueryContainer> WrapAsync<T>(this Task<CallbackQuery> message,
                                            IContainer<T> containerAbs) where T : class
            => containerAbs.RebaseContainer(await message);

        public static object GetInnerUpdate(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}");
        }

        public static T GetInnerUpdate<T>(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return (T)(typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}"));
        }

        public static UpdateType? GetUpdateType<T>()
        {
            if (Enum.TryParse(typeof(T).Name, out UpdateType result))
            {
                return result;
            }

            return null;
        }
    }
}
