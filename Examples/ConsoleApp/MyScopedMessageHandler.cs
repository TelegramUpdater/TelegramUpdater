using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace ConsoleApp;

[ApplyFilter(typeof(PrivateTestCommand))]
internal class MyScopedMessageHandler : ScopedMessageHandler
{
    public MyScopedMessageHandler() : base(group: 0)
    { }

    protected override async Task HandleAsync(UpdateContainerAbs<Message> container)
    {
        await container.Response("Tested!");
    }
}

internal class PrivateTestCommand : Filter<Message>
{
    public PrivateTestCommand()
        : base(FilterCutify.OnCommand("test") & FilterCutify.PM())
    {
    }
}
