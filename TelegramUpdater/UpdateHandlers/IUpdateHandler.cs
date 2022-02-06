using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers
{
    public interface IUpdateHandler
    {
        public int Group { get; }

        public Task HandleAsync(Updater updater, ITelegramBotClient botClient, Update update);
    }
}
