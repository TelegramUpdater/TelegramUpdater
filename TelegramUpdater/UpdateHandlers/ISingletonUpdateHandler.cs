using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers
{
    public interface ISingletonUpdateHandler : IUpdateHandler
    {
        public bool ShouldHandle(Update update);
    }
}
