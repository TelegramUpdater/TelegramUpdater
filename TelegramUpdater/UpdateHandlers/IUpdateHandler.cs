using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers
{
    public interface IUpdateHandler
    {
        public int Group { get; }

        public Task HandleAsync(IUpdater updater, Update update);
    }
}
