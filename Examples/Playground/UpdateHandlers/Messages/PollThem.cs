using Telegram.Bot;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

internal class PollThem : MessageHandler
{
    protected override async Task HandleAsync()
    {
        //BotClient.SendPoll(Container.GetChatId())
    }
}
