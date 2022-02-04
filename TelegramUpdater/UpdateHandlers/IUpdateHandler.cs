using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers
{
    public interface IUpdateHandler
    {
        public bool ShouldHandle(Update update);

        public Task HandleAsync(Updater updater, ITelegramBotClient botClient, Update update);
    }
}
