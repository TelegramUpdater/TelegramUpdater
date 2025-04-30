using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("rename"), Private]
internal class Rename : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> cntr)
    {
        if (From is null) return;

        await Response(
            "Ok let's rename you! What is your name?",
            replyMarkup: new ForceReplyMarkup());

        InitiateState<RenameState>(From);
    }
}

[Text, Renameing(RenameState.AskingName), Private]
internal class RenameAskName : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> cntr)
    {
        if (From is null) return;

        switch (ActualUpdate)
        {
            case { Text: { } text } when !string.IsNullOrWhiteSpace(text):                
                await cntr.Response(
                    $"Ok {text}, what is your last name?",
                    replyMarkup: new ForceReplyMarkup());

                ForwardState<RenameState>(From);

                break;
                
            default:                
                await cntr.Response("Please provide a name.");
                break;
        }
    }
}

[Text, Renameing(RenameState.AskingLastName), Private]
internal class RenameAskLastName : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> cntr)
    {
        if (From is null) return;

        switch (ActualUpdate)
        {
            case { Text: { } text } when !string.IsNullOrWhiteSpace(text):
                await cntr.Response(
                    $"Ok {text}, you are now renamed!",
                    replyMarkup: new ReplyKeyboardRemove());

                DeleteState<RenameState>(From);

                break;
                
            default:                
                await cntr.Response("Please provide a last name.");
                break;
                
        }
    }
}



internal enum RenameState
{
    AskingName,
    AskingLastName,
}

internal class RenameingAttribute(RenameState state)
    : UserEnumStateAttribute<RenameState>(state);