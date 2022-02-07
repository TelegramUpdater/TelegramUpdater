using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    public abstract class ScopedMessageHandler : AnyScopedHandler<Message>
    {
        protected ScopedMessageHandler(int group = 0) : base(x => x.Message, group)
        {
        }
    }
}
