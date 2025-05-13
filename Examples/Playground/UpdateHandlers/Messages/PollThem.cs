using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Playground.UpdateHandlers.Messages;

[Command("poll")]
internal class PollThem : MessageHandler
{
    protected override async Task HandleAsync()
    {
        var senderId = Container.GetSenderId().ContinueIfNull();
        var chat = Container.GetChatId().ContinueIfNull();

        var msg = await BotClient.SendPoll(
            chat,
            "How much do you like this package",
            [
                new InputPollOption("Yes 😍"),
                new InputPollOption("I'm not sure 😶"),
                new InputPollOption("Not at all 😫")],
            isAnonymous: false,
            type: PollType.Regular,
            allowsMultipleAnswers: false,
            openPeriod: 30);

        var answer = await Container.OpenChannel(
            TimeSpan.FromSeconds(30),
            new UpdaterFilter<PollAnswer>(
                (PollAnswer pollAnswer) => pollAnswer.User?.Id == senderId));

        if (answer != null)
        {
            await BotClient.StopPoll(chat, msg.Id);
            await Response($"You've chosen, Option {answer.Update.OptionIds[0]}.");
        }
        else
        {
            await Response("Quicker!");
        }
    }
}
