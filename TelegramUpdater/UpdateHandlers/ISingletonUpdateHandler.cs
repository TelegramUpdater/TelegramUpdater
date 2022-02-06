using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers
{
    public interface ISingletonUpdateHandler : IUpdateHandler
    {
        UpdateType UpdateType { get; }

        public bool ShouldHandle(Update update);
    }
}
