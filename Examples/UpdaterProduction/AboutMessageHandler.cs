using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace UpdaterProduction;

[Command("about"), Private]
internal class AboutMessageHandler : ScopedMessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> container)
    {
        await container.Response($"*How about you?", parseMode: ParseMode.Markdown);
    }
}
