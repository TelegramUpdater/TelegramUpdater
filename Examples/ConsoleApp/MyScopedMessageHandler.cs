using Telegram.Bot.Types;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace ConsoleApp;

[Command("test"), ChatType(ChatTypeFlags.Private)]
internal class MyScopedMessageHandler : MessageHandler
{
    public MyScopedMessageHandler() : base(group: 0)
    { }

    protected override async Task HandleAsync(IContainer<Message> container)
    {
        await ResponseAsync("Tested!");
    }
}
