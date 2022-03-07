using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace UpdaterProduction;

[Command("about"), Private]
internal class AboutMessageHandler : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> container)
    {
        await ResponseAsync($"*How about you?", parseMode: ParseMode.Markdown);
    }
}
