using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse
{
    /// <summary>
    /// Abstarct <see cref="IScopedUpdateHandler"/> for <see cref="Update.Message"/>.
    /// </summary>
    public abstract class ScopedMessageHandler : AnyScopedHandler<Message>
    {
        protected ScopedMessageHandler(int group = 0) : base(x => x.Message, group)
        {
        }
    }
}
