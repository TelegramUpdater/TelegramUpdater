// Ignore Spelling: cntr

using Telegram.Bot.Types;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command(command: "start"), Private]
internal class Start : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> cntr)
    {
        await ResponseAsync("Hi there!");
        await Task.Delay(5000);
    }
}
